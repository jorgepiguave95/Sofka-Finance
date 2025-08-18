using Customers.Application.Interfaces;
using Customers.Application.Repositories;
using Customers.Domain.Entities;
using SofkaFinance.Contracts.Customers;
using BCrypt.Net;
using Customers.Domain.Errors;

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
            Console.WriteLine($"[CustomerService] Procesando cliente: {command.Name}");

            // Test temporal: simular error del sistema si el nombre contiene "TEST_ERROR"
            if (command.Name.Contains("TEST_ERROR"))
            {
                Console.WriteLine($"[CustomerService] ¡SIMULANDO EXCEPCIÓN! para nombre: {command.Name}");
                throw new InvalidOperationException("Error simulado del sistema para pruebas");
            }

            // Validar si ya existe un cliente con la misma identificación
            var existsByIdentification = await _clientRepository.ExistsByIdentificationAsync(command.Identification);
            if (existsByIdentification)
            {
                return new CreateCustomerResponse(
                    OperationId: command.OperationId,
                    Message: $"Ya existe un cliente con la identificación '{command.Identification}'"
                );
            }

            // Validar si ya existe un cliente con el mismo email
            var existsByEmail = await _clientRepository.ExistsByEmailAsync(command.Email);
            if (existsByEmail)
            {
                return new CreateCustomerResponse(
                    OperationId: command.OperationId,
                    Message: $"Ya existe un cliente con el correo electrónico '{command.Email}'"
                );
            }

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
        catch (DomainException ex)
        {
            // Errores de dominio (validaciones de reglas de negocio)
            Console.WriteLine($"[CustomerService] DomainException: {ex.Message}");
            return new CreateCustomerResponse(
                OperationId: command.OperationId,
                Message: ex.Message
            );
        }
        catch (Exception ex)
        {
            // Otros errores inesperados - ATRAPAMOS CUALQUIER EXCEPCIÓN
            Console.WriteLine($"[CustomerService] Exception no controlada: {ex.Message}");
            Console.WriteLine($"[CustomerService] StackTrace: {ex.StackTrace}");

            return new CreateCustomerResponse(
                OperationId: command.OperationId,
                Message: $"Error interno del servidor: {ex.Message}"
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

            // Validar si el nuevo email ya existe en otro cliente
            if (!string.IsNullOrWhiteSpace(command.Email) && command.Email != client.Email.Value)
            {
                var existsByEmail = await _clientRepository.ExistsByEmailAsync(command.Email);
                if (existsByEmail)
                {
                    return new UpdateCustomerResponse(
                        OperationId: command.OperationId,
                        Message: $"Ya existe otro cliente con el correo electrónico '{command.Email}'"
                    );
                }
            }

            client.ChangeEmail(command.Email);

            await _clientRepository.UpdateAsync(client);

            return new UpdateCustomerResponse(
                OperationId: command.OperationId,
                Message: "Cliente actualizado exitosamente",
                CustomerId: client.Id
            );
        }
        catch (DomainException ex)
        {
            // Errores de dominio (validaciones de reglas de negocio)
            return new UpdateCustomerResponse(
                OperationId: command.OperationId,
                Message: ex.Message
            );
        }
        catch (Exception)
        {
            // Otros errores inesperados
            return new UpdateCustomerResponse(
                OperationId: command.OperationId,
                Message: "Error interno del servidor. Por favor, intente nuevamente."
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
        catch (DomainException ex)
        {
            // Errores de dominio (validaciones de reglas de negocio)
            return new DeleteCustomerResponse(
                OperationId: command.OperationId,
                Message: ex.Message
            );
        }
        catch (Exception)
        {
            // Otros errores inesperados
            return new DeleteCustomerResponse(
                OperationId: command.OperationId,
                Message: "Error interno del servidor. Por favor, intente nuevamente."
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

            var customerData = new CustomerData(
                Id: client.Id,
                Name: client.Name ?? "NULL_NAME",
                Gender: client.Gender?.Value ?? "NULL_GENDER",
                Age: client.Age?.Value ?? 0,
                Identification: client.Identification ?? "NULL_ID",
                Address: client.Address?.Value ?? "NULL_ADDRESS",
                Phone: client.Phone?.Value ?? "NULL_PHONE",
                Email: client.Email?.Value ?? "NULL_EMAIL",
                IsActive: client.IsActive,
                CreatedAt: client.CreatedAt,
                UpdatedAt: client.UpdatedAt
            );

            Console.WriteLine($"[CustomerService] Client loaded: Name={client.Name}, Gender={client.Gender?.Value}, Email={client.Email?.Value}");

            return new GetCustomerResponse(
                OperationId: query.OperationId,
                Message: "Cliente encontrado exitosamente",
                Customer: customerData
            );
        }
        catch (DomainException ex)
        {
            // Errores de dominio (validaciones de reglas de negocio)
            return new GetCustomerResponse(
                OperationId: query.OperationId,
                Message: ex.Message
            );
        }
        catch (Exception)
        {
            // Otros errores inesperados
            return new GetCustomerResponse(
                OperationId: query.OperationId,
                Message: "Error interno del servidor. Por favor, intente nuevamente."
            );
        }
    }

    public async Task<GetAllCustomersResponse> GetAllAsync(GetAllCustomersQuery query)
    {
        try
        {
            var clients = await _clientRepository.GetAllAsync();

            var customerData = clients.Select(client => new CustomerData(
                Id: client.Id,
                Name: client.Name ?? "",
                Gender: client.Gender?.Value ?? "",
                Age: client.Age?.Value ?? 0,
                Identification: client.Identification ?? "",
                Address: client.Address?.Value ?? "",
                Phone: client.Phone?.Value ?? "",
                Email: client.Email?.Value ?? "",
                IsActive: client.IsActive,
                CreatedAt: client.CreatedAt,
                UpdatedAt: client.UpdatedAt
            )).ToArray();

            return new GetAllCustomersResponse(
                OperationId: query.OperationId,
                Message: "Clientes obtenidos exitosamente",
                Customers: customerData
            );
        }
        catch (DomainException ex)
        {
            // Errores de dominio (validaciones de reglas de negocio)
            return new GetAllCustomersResponse(
                OperationId: query.OperationId,
                Message: ex.Message,
                Customers: Array.Empty<CustomerData>()
            );
        }
        catch (Exception)
        {
            // Otros errores inesperados
            return new GetAllCustomersResponse(
                OperationId: query.OperationId,
                Message: "Error interno del servidor. Por favor, intente nuevamente.",
                Customers: Array.Empty<CustomerData>()
            );
        }
    }
}
