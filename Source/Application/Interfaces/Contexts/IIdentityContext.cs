using Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces.Contexts;

public interface IIdentityContext
{
    DbSet<User> Users { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
