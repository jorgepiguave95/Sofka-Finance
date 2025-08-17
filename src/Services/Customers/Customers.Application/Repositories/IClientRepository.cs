using Customers.Domain.Entities;

namespace Customers.Application.Repositories;

public interface IClientRepository
{
    Task<Client?> GetByIdAsync(Guid id);
    Task<Client?> GetByEmailAsync(string email);
    Task<Client?> GetByIdentificationAsync(string identification);
    Task<IEnumerable<Client>> GetAllAsync();
    Task<Client> CreateAsync(Client client);
    Task UpdateAsync(Client client);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> ExistsByIdentificationAsync(string identification);
}
