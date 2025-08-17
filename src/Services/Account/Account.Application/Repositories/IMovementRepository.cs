using Account.Domain.Entities;

namespace Account.Application.Repositories;

public interface IMovementRepository
{
    Task<Movement?> GetByIdAsync(Guid id);
    Task<IEnumerable<Movement>> GetByAccountIdAsync(Guid accountId);
    Task<IEnumerable<Movement>> GetByAccountIdAndDateRangeAsync(
        Guid accountId,
        DateTime startDate,
        DateTime endDate);
    Task<Movement> CreateAsync(Movement movement);
    Task UpdateAsync(Movement movement);
    Task<bool> ExistsAsync(Guid id);
}