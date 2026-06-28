using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Core.ValueObjects;
using Braavo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Braavo.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly BraavoDbContext _context;

    public UserRepository(BraavoDbContext context) => _context = context;

    public async Task<User?> GetByIdAsync(UserId id, CancellationToken ct = default)
        => await _context.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<User?> GetByEmailAsync(Email email, CancellationToken ct = default)
        => await _context.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        await _context.Users.AddAsync(user, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(User user, CancellationToken ct = default)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync(ct);
    }
}
