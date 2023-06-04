using Domain.Entities.Identity;

namespace Application.Interfaces.Services;

public interface IJwtService
{
    string GenerateToken(User user);
}
