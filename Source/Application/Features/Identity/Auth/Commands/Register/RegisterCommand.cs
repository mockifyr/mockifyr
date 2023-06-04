using MediatR;

namespace Application.Features.Identity.Auth.Commands.Register;

public class RegisterCommand : IRequest<bool>
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, bool>
{
    public async Task<bool> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
