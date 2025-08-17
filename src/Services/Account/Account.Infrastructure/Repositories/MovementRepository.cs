using Microsoft.EntityFrameworkCore;
using Account.Application.Repositories;
using Account.Domain.Entities;
using Account.Infrastructure.Persistence;

namespace Account.Infrastructure.Repositories;

public class MovementRepository : IMovementRepository
{
    private readonly AccountDbContext _context;

    public MovementRepository(AccountDbContext context)
    {
        _context = context;
    }

    public async Task<Movement?> GetByIdAsync(Guid id)
    {
        return await _context.Movements
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IEnumerable<Movement>> GetByAccountIdAsync(Guid accountId)
    {
        return await _context.Movements
            .Where(m => m.AccountId == accountId)
            .OrderByDescending(m => m.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Movement>> GetByAccountIdAndDateRangeAsync(
        Guid accountId,
        DateTime startDate,
        DateTime endDate)
    {
        return await _context.Movements
            .Where(m => m.AccountId == accountId
                && m.Date >= startDate
                && m.Date <= endDate)
            .OrderByDescending(m => m.Date)
            .ToListAsync();
    }

    public async Task<Movement> CreateAsync(Movement movement)
    {
        _context.Movements.Add(movement);
        await _context.SaveChangesAsync();
        return movement;
    }

    public async Task UpdateAsync(Movement movement)
    {
        _context.Movements.Update(movement);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Movements.AnyAsync(m => m.Id == id);
    }
}
