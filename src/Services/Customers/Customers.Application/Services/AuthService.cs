using Customers.Application.Interfaces;
using Customers.Application.Repositories;
using SofkaFinance.Contracts.Customers;
using BCrypt.Net;

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
            var client = await _clientRepository.GetByEmailAsync(command.Email);

            if (client == null)
            {
                return new LoginResponse(
                    OperationId: command.OperationId,
                    Message: "Credenciales inválidas"
                );
            }

            if (!client.IsActive)
            {
                return new LoginResponse(
                    OperationId: command.OperationId,
                    Message: "La cliente está desactivado"
                );
            }

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
        catch (Exception ex)
        {
            return new LoginResponse(
                OperationId: command.OperationId,
                Message: ex.Message
            );
        }
    }
}
