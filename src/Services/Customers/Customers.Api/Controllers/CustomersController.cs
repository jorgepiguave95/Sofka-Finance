using Customers.Application.Interfaces;
using SofkaFinance.Contracts.Customers;

namespace Customers.Api.Controllers;

public class CustomersController
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public async Task<GetCustomerResponse> GetById(GetCustomerByIdQuery query)
    {
        return await _customerService.GetByIdAsync(query);
    }

    public async Task<GetAllCustomersResponse> GetAll(GetAllCustomersQuery query)
    {
        return await _customerService.GetAllAsync(query);
    }

    public async Task<CreateCustomerResponse> Create(CreateCustomerCommand command)
    {
        return await _customerService.CreateAsync(command);
    }
    public async Task<UpdateCustomerResponse> Update(UpdateCustomerCommand command)
    {
        return await _customerService.UpdateAsync(command);
    }

    public async Task<DeleteCustomerResponse> Delete(DeleteCustomerCommand command)
    {
        return await _customerService.DeleteAsync(command);
    }
}