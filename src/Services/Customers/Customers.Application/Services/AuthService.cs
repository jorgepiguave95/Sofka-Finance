using Customers.Application.Interfaces;
using Customers.Application.Repositories;
using SofkaFinance.Contracts.Customers;
using BCrypt.Net;
using Customers.Domain.Errors;

namespace Customers.Application.Services;

public class AuthService : IAuthService
{
    private readonly IClientRepository _clientRepository;

    public AuthService(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<LoginResponse> LoginAsync(LoginCommand command)
    {
        try
        {
            // Validar que el email no esté vacío
            if (string.IsNullOrWhiteSpace(command.Email))
            {
                return new LoginResponse(
                    OperationId: command.OperationId,
                    Message: "El correo electrónico es requerido"
                );
            }

            // Validar que la contraseña no esté vacía
            if (string.IsNullOrWhiteSpace(command.Password))
            {
                return new LoginResponse(
                    OperationId: command.OperationId,
                    Message: "La contraseña es requerida"
                );
            }

            var client = await _clientRepository.GetByEmailAsync(command.Email);

            if (client == null)
            {
                return new LoginResponse(
                    OperationId: command.OperationId,
                    Message: "Credenciales inválidas"
                );
            }

            // Ya no necesitamos validar IsActive aquí porque el repositorio ya lo hace

            if (!BCrypt.Net.BCrypt.Verify(command.Password, client.Password.Value))
            {
                return new LoginResponse(
                    OperationId: command.OperationId,
                    Message: "Credenciales inválidas"
                );
            }

            var token = "Token-JWT";

            return new LoginResponse(
                OperationId: command.OperationId,
                Message: "Inicio de sesión exitoso",
                Token: token,
                CustomerId: client.Id,
                CustomerEmail: client.Email.Value
            );
        }
        catch (DomainException ex)
        {
            // Errores de dominio (validaciones de reglas de negocio)
            return new LoginResponse(
                OperationId: command.OperationId,
                Message: ex.Message
            );
        }
        catch (Exception)
        {
            // Otros errores inesperados
            return new LoginResponse(
                OperationId: command.OperationId,
                Message: "Error interno del servidor. Por favor, intente nuevamente."
            );
        }
    }
}
