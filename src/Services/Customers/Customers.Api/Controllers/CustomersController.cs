using Customers;
using SofkaFinance.Contracts.Customers;

namespace Customers.Api.Controllers;

public class CustomersController
{
    public async Task<GetCustomerResponse> GetById(GetCustomerByIdQuery query)
    {
        try
        {
            var customer = new
            {
                Id = query.CustomerId,
                Name = "Cliente Ejemplo",
                Email = "cliente@ejemplo.com",
                Phone = "123456789",
                Address = "Calle Falsa 123",
                Identification = "ABC123456",
                Gender = "Masculino",
                Age = 30,
                CreatedAt = DateTime.UtcNow.AddYears(-1)
            };
            await Task.CompletedTask;
            return new GetCustomerResponse(
                OperationId: query.OperationId,
                Success: true,
                Message: "Cliente encontrado",
                Customer: customer
            );
        }
        catch (Exception ex)
        {
            return new GetCustomerResponse(
                OperationId: query.OperationId,
                Success: false,
                Message: $"Error: {ex.Message}",
                Customer: null
            );
        }
    }

    public async Task<GetAllCustomersResponse> GetAll(GetAllCustomersQuery query)
    {
        try
        {
            var customers = new[]
            {
                new {
                    Id = Guid.NewGuid(),
                    Name = "Cliente Uno",
                    Email = "uno@ejemplo.com",
                    Phone = "111111111",
                    Address = "Calle 1",
                    Identification = "ID1",
                    Gender = "Femenino",
                    Age = 25,
                    CreatedAt = DateTime.UtcNow.AddYears(-2)
                },
                new {
                    Id = Guid.NewGuid(),
                    Name = "Cliente Dos",
                    Email = "dos@ejemplo.com",
                    Phone = "222222222",
                    Address = "Calle 2",
                    Identification = "ID2",
                    Gender = "Masculino",
                    Age = 35,
                    CreatedAt = DateTime.UtcNow.AddYears(-3)
                }
            };
            await Task.CompletedTask;

            return new GetAllCustomersResponse(
                OperationId: query.OperationId,
                Success: true,
                Message: "Lista de clientes obtenida",
                Customers: customers.Cast<object>().ToArray()
            );
        }
        catch (Exception ex)
        {
            return new GetAllCustomersResponse(
                OperationId: query.OperationId,
                Success: false,
                Message: $"Error: {ex.Message}",
                Customers: null
            );
        }
    }

    public async Task<CreateCustomerResponse> Create(CreateCustomerCommand command)
    {
        try
        {
            var newId = Guid.NewGuid();
            await Task.CompletedTask;
            return new CreateCustomerResponse(
                OperationId: command.OperationId,
                Success: true,
                Message: "Cliente creado exitosamente",
                CustomerId: newId
            );
        }
        catch (Exception ex)
        {
            return new CreateCustomerResponse(
                OperationId: command.OperationId,
                Success: false,
                Message: $"Error: {ex.Message}",
                CustomerId: null
            );
        }
    }

    public async Task<UpdateCustomerResponse> Update(UpdateCustomerCommand command)
    {
        try
        {
            await Task.CompletedTask;
            return new UpdateCustomerResponse(
                OperationId: command.OperationId,
                Success: true,
                Message: "Cliente actualizado exitosamente",
                CustomerId: command.CustomerId
            );
        }
        catch (Exception ex)
        {
            return new UpdateCustomerResponse(
                OperationId: command.OperationId,
                Success: false,
                Message: $"Error: {ex.Message}",
                CustomerId: null
            );
        }
    }

    public async Task<DeleteCustomerResponse> Delete(DeleteCustomerCommand command)
    {
        try
        {
            await Task.CompletedTask;
            return new DeleteCustomerResponse(
                OperationId: command.OperationId,
                Success: true,
                Message: "Cliente eliminado exitosamente",
                CustomerId: command.CustomerId
            );
        }
        catch (Exception ex)
        {
            return new DeleteCustomerResponse(
                OperationId: command.OperationId,
                Success: false,
                Message: $"Error: {ex.Message}",
                CustomerId: null
            );
        }
    }
}
