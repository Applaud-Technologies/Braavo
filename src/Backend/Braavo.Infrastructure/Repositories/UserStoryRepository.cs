using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Braavo.Infrastructure.Repositories;

public class UserStoryRepository : IUserStoryRepository
{
    private readonly BraavoDbContext _context;

    public UserStoryRepository(BraavoDbContext context)
    {
        _context = context;
    }

    public async Task<UserStory?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.UserStories.FirstOrDefaultAsync(s => s.Id == id, ct);
    }

    public async Task<IReadOnlyList<UserStory>> GetByProductIdAsync(Guid productId, CancellationToken ct = default)
    {
        return await _context.UserStories
            .Where(s => s.ProductId == productId)
            .OrderBy(s => s.Priority)
            .ThenBy(s => s.SortOrder)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<UserStory>> GetByPersonaIdAsync(Guid personaId, CancellationToken ct = default)
    {
        return await _context.UserStories
            .Where(s => s.PersonaId == personaId)
            .OrderBy(s => s.SortOrder)
            .ToListAsync(ct);
    }

    public async Task AddAsync(UserStory story, CancellationToken ct = default)
    {
        await _context.UserStories.AddAsync(story, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(UserStory story, CancellationToken ct = default)
    {
        _context.UserStories.Update(story);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var story = await GetByIdAsync(id, ct);
        if (story is not null)
        {
            _context.UserStories.Remove(story);
            await _context.SaveChangesAsync(ct);
        }
    }
}
