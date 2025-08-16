using Customers;
using SofkaFinance.Contracts.Customers;

namespace Customers.Api.Controllers;

public class CustomersController
{
    public Task GetById(SofkaFinance.Contracts.Customers.GetCustomerByIdQuery query)
    {
        Console.WriteLine($"GetById query processed for CustomerId: {query.CustomerId}");
        return Task.CompletedTask;
    }

    public Task GetAll(SofkaFinance.Contracts.Customers.GetAllCustomersQuery query)
    {
        Console.WriteLine($"GetAll customers query processed with OperationId: {query.OperationId}");
        return Task.CompletedTask;
    }

    public Task Search(SofkaFinance.Contracts.Customers.SearchCustomersQuery query)
    {
        Console.WriteLine($"Search customers query processed: {query.SearchTerm}");
        return Task.CompletedTask;
    }

    public Task Create(SofkaFinance.Contracts.Customers.CreateCustomerCommand command)
    {
        Console.WriteLine($"Customer created: {command.Name} with OperationId: {command.OperationId}");
        return Task.CompletedTask;
    }

    public Task Update(SofkaFinance.Contracts.Customers.UpdateCustomerCommand command)
    {
        Console.WriteLine($"Customer updated: {command.Name} for CustomerId: {command.CustomerId}");
        return Task.CompletedTask;
    }

    public Task Delete(SofkaFinance.Contracts.Customers.DeleteCustomerCommand command)
    {
        Console.WriteLine($"Customer deleted for CustomerId: {command.CustomerId}");
        return Task.CompletedTask;
    }

    public Task Activate(SofkaFinance.Contracts.Customers.ActivateCustomerCommand command)
    {
        Console.WriteLine($"Customer activated for CustomerId: {command.CustomerId}");
        return Task.CompletedTask;
    }

    public Task Deactivate(SofkaFinance.Contracts.Customers.DeactivateCustomerCommand command)
    {
        Console.WriteLine($"Customer deactivated for CustomerId: {command.CustomerId}");
        return Task.CompletedTask;
    }
}
