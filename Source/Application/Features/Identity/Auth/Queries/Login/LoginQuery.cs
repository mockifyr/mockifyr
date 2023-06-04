using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities.Identity;
using Domain.Wrappers;
using MediatR;

namespace Application.Features.Identity.Auth.Queries.Login;

public class LoginQuery : IRequest<ResponseContainer<LoginQueryResponse>>
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class LoginQueryResponse
{
    public User User { get; set; }
    public string Token { get; set; }
}

public class LoginQueryHandler : IRequestHandler<LoginQuery, ResponseContainer<LoginQueryResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public LoginQueryHandler(IUserRepository userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<ResponseContainer<LoginQueryResponse>> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        // Find User
        User user = await _userRepository.FindByEmailAsync(request.Email);
        if (user == null)
        {
            // Return Unsuccessful Response
            return new ResponseContainer<LoginQueryResponse>(false, "0", "Kullanıcı bulunamadı.");
        }

        // Check Password
        var isPasswordValid = await _userRepository.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
        {
            // Return Unsuccessful Response
            return new ResponseContainer<LoginQueryResponse>(false, "1", "Geçersiz parola.");
        }

        // Generate Token and Create Valid Response
        LoginQueryResponse loginQueryResponse = new() { User = user, Token = _jwtService.GenerateToken(user)};

        // Return Success Response
        return new ResponseContainer<LoginQueryResponse>(loginQueryResponse, true);
    }
}