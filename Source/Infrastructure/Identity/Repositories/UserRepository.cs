using Application.Interfaces.Contexts;
using Application.Interfaces.Repositories;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Identity.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IIdentityContext _context;

    public UserRepository(IIdentityContext context)
    {
        _context = context;
    }

    public async Task<bool> CheckPasswordAsync(User user, string password)
    {
        var passwordHasher = new PasswordHasher<User>();
        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

        return result == PasswordVerificationResult.Success;
    }

    public async Task CreateUserAsync(User user, string password)
    {
        var passwordHasher = new PasswordHasher<User>();
        user.PasswordHash = passwordHasher.HashPassword(user, password);

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task<User> FindByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }
}
