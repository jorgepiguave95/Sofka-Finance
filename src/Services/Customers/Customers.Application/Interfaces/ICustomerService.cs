using SofkaFinance.Contracts.Customers;

namespace Customers.Application.Interfaces;

public interface ICustomerService
{
    Task<CreateCustomerResponse> CreateAsync(CreateCustomerCommand command);
    Task<UpdateCustomerResponse> UpdateAsync(UpdateCustomerCommand command);
    Task<DeleteCustomerResponse> DeleteAsync(DeleteCustomerCommand command);
    Task<GetCustomerResponse> GetByIdAsync(GetCustomerByIdQuery query);
    Task<GetAllCustomersResponse> GetAllAsync(GetAllCustomersQuery query);
}
