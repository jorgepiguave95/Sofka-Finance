using Microsoft.EntityFrameworkCore;
using Customers.Application.Repositories;
using Customers.Infrastructure.Persistence;
using Customers.Domain.Entities;

namespace Customers.Infrastructure.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly CustomersDbContext _context;

    public ClientRepository(CustomersDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Client?> GetByIdAsync(Guid id)
    {
        return await _context.Clients
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Client?> GetByEmailAsync(string email)
    {
        return await _context.Clients
            .FirstOrDefaultAsync(c => c.Email.Value == email);
    }

    public async Task<Client?> GetByIdentificationAsync(string identification)
    {
        return await _context.Clients
            .FirstOrDefaultAsync(c => c.Identification == identification);
    }

    public async Task<IEnumerable<Client>> GetAllAsync()
    {
        return await _context.Clients
            .Where(c => c.IsActive)
            .ToListAsync();
    }

    public async Task<Client> CreateAsync(Client client)
    {
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();
        return client;
    }

    public async Task UpdateAsync(Client client)
    {
        _context.Clients.Update(client);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Clients
            .AnyAsync(c => c.Id == id);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Clients
            .AnyAsync(c => c.Email.Value == email);
    }

    public async Task<bool> ExistsByIdentificationAsync(string identification)
    {
        return await _context.Clients
            .AnyAsync(c => c.Identification == identification);
    }
}
