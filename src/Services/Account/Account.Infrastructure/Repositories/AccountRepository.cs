using Microsoft.EntityFrameworkCore;
using Account.Application.Repositories;
using Account.Infrastructure.Persistence;
using AccountEntity = Account.Domain.Entities.Account;

namespace Account.Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly AccountDbContext _context;

    public AccountRepository(AccountDbContext context)
    {
        _context = context;
    }

    public async Task<AccountEntity?> GetByIdAsync(Guid id)
    {
        return await _context.Accounts
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<AccountEntity?> GetByNumberAsync(string accountNumber)
    {
        return await _context.Accounts
            .FirstOrDefaultAsync(a => a.Number.Value == accountNumber);
    }

    public async Task<IEnumerable<AccountEntity>> GetByCustomerIdAsync(Guid customerId)
    {
        return await _context.Accounts
            .Where(a => a.CustomerId == customerId)
            .ToListAsync();
    }

    public async Task<IEnumerable<AccountEntity>> GetAllAsync()
    {
        return await _context.Accounts
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<AccountEntity> CreateAsync(AccountEntity account)
    {
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();
        return account;
    }

    public async Task UpdateAsync(AccountEntity account)
    {
        _context.Accounts.Update(account);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Accounts.AnyAsync(a => a.Id == id);
    }

    public async Task<string> GenerateAccountNumberAsync()
    {
        string accountNumber;
        do
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var random = new Random().Next(1000, 9999);
            accountNumber = $"{timestamp}{random}";
        }
        while (await _context.Accounts.AnyAsync(a => a.Number.Value == accountNumber));

        return accountNumber;
    }
}