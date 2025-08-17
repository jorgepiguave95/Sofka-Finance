using Customers.Application.Interfaces;
using Customers.Application.Repositories;
using Customers.Domain.Entities;
using SofkaFinance.Contracts.Customers;
using BCrypt.Net;

namespace Customers.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly IClientRepository _clientRepository;

    public CustomerService(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<CreateCustomerResponse> CreateAsync(CreateCustomerCommand command)
    {
        try
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(command.Password);

            var client = Client.Create(
                name: command.Name,
                gender: command.Gender,
                age: command.Age,
                identification: command.Identification,
                address: command.Address,
                phone: command.Phone,
                email: command.Email,
                password: hashedPassword
            );

            var createdClient = await _clientRepository.CreateAsync(client);

            return new CreateCustomerResponse(
                OperationId: command.OperationId,
                Message: "Cliente creado exitosamente",
                CustomerId: createdClient.Id
            );
        }
        catch (Exception ex)
        {
            return new CreateCustomerResponse(
                OperationId: command.OperationId,
                Message: ex.Message
            );
        }
    }

    public async Task<UpdateCustomerResponse> UpdateAsync(UpdateCustomerCommand command)
    {
        try
        {
            var client = await _clientRepository.GetByIdAsync(command.CustomerId);
            if (client == null)
            {
                return new UpdateCustomerResponse(
                    OperationId: command.OperationId,
                    Message: "Cliente no encontrado"
                );
            }

            client.ChangeEmail(command.Email);

            await _clientRepository.UpdateAsync(client);

            return new UpdateCustomerResponse(
                OperationId: command.OperationId,
                Message: "Cliente actualizado exitosamente",
                CustomerId: client.Id
            );
        }
        catch (Exception ex)
        {
            return new UpdateCustomerResponse(
                OperationId: command.OperationId,
                Message: ex.Message
            );
        }
    }

    public async Task<DeleteCustomerResponse> DeleteAsync(DeleteCustomerCommand command)
    {
        try
        {
            var client = await _clientRepository.GetByIdAsync(command.CustomerId);
            if (client == null)
            {
                return new DeleteCustomerResponse(
                    OperationId: command.OperationId,
                    Message: "Cliente no encontrado"
                );
            }

            client.Deactivate();
            await _clientRepository.UpdateAsync(client);

            return new DeleteCustomerResponse(
                OperationId: command.OperationId,
                Message: "Cliente desactivado exitosamente"
            );
        }
        catch (Exception ex)
        {
            return new DeleteCustomerResponse(
                OperationId: command.OperationId,
                Message: ex.Message
            );
        }
    }

    public async Task<GetCustomerResponse> GetByIdAsync(GetCustomerByIdQuery query)
    {
        try
        {
            var client = await _clientRepository.GetByIdAsync(query.CustomerId);
            if (client == null)
            {
                return new GetCustomerResponse(
                    OperationId: query.OperationId,
                    Message: "Cliente no encontrado"
                );
            }

            var customerObject = new
            {
                Id = client.Id,
                Name = client.Name,
                Gender = client.Gender,
                Age = client.Age.Value,
                Identification = client.Identification,
                Address = client.Address.Value,
                Phone = client.Phone.Value,
                Email = client.Email.Value,
                IsActive = client.IsActive,
                CreatedAt = client.CreatedAt,
                UpdatedAt = client.UpdatedAt
            };

            return new GetCustomerResponse(
                OperationId: query.OperationId,
                Message: "Cliente encontrado exitosamente",
                Customer: customerObject
            );
        }
        catch (Exception ex)
        {
            return new GetCustomerResponse(
                OperationId: query.OperationId,
                Message: ex.Message
            );
        }
    }

    public async Task<GetAllCustomersResponse> GetAllAsync(GetAllCustomersQuery query)
    {
        try
        {
            var clients = await _clientRepository.GetAllAsync();

            var customerObjects = clients.Select(client => new
            {
                Id = client.Id,
                Name = client.Name,
                Gender = client.Gender,
                Age = client.Age.Value,
                Identification = client.Identification,
                Address = client.Address.Value,
                Phone = client.Phone.Value,
                Email = client.Email.Value,
                IsActive = client.IsActive,
                CreatedAt = client.CreatedAt,
                UpdatedAt = client.UpdatedAt
            }).ToArray();

            return new GetAllCustomersResponse(
                OperationId: query.OperationId,
                Message: "Clientes obtenidos exitosamente",
                Customers: customerObjects
            );
        }
        catch (Exception ex)
        {
            return new GetAllCustomersResponse(
                OperationId: query.OperationId,
                Message: ex.Message,
                Customers: Array.Empty<object>()
            );
        }
    }
}
