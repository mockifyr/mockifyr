using Domain.Entities.Identity;

namespace Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<bool> CheckPasswordAsync(User user, string password);
    Task CreateUserAsync(User user, string password);
    Task<User> FindByEmailAsync(string email);
}
