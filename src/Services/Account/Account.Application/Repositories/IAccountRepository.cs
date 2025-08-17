namespace Account.Application.Repositories;

public interface IAccountRepository
{
    Task<Domain.Entities.Account?> GetByIdAsync(Guid id);
    Task<Domain.Entities.Account?> GetByNumberAsync(string accountNumber);
    Task<IEnumerable<Domain.Entities.Account>> GetByCustomerIdAsync(Guid customerId);
    Task<IEnumerable<Domain.Entities.Account>> GetAllAsync();
    Task<Domain.Entities.Account> CreateAsync(Domain.Entities.Account account);
    Task UpdateAsync(Domain.Entities.Account account);
    Task<bool> ExistsAsync(Guid id);
    Task<string> GenerateAccountNumberAsync();
}