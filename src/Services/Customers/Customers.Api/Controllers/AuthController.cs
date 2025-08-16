using Customers;
using SofkaFinance.Contracts.Customers;

namespace Customers.Api.Controllers;

public class AuthController
{
    public async Task<LoginResponse> Login(LoginCommand command)
    {
        try
        {
            await Task.Delay(1);

            var validUsers = new[]
            {
                new { Email = "admin@sofka.com", Password = "admin123", Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Administrador" },
                new { Email = "usuario@test.com", Password = "password123", Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Usuario Test" },
                new { Email = "cliente@ejemplo.com", Password = "cliente123", Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Cliente Ejemplo" }
            };

            var user = validUsers.FirstOrDefault(u =>
                u.Email.Equals(command.Email, StringComparison.OrdinalIgnoreCase) &&
                u.Password == command.Password);

            if (user == null)
            {
                return new LoginResponse(
                    OperationId: command.OperationId,
                    Success: false,
                    Message: "Credenciales inválidas"
                );
            }

            var token = $"SFK_TOKEN_{user.Id:N}_{user.Email.Replace("@", "_")}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
            var expiresAt = DateTime.UtcNow.AddHours(24);

            return new LoginResponse(
                OperationId: command.OperationId,
                Success: true,
                Message: "Inicio de sesión exitoso",
                Token: token,
                CustomerId: user.Id,
                CustomerEmail: user.Email,
                ExpiresAt: expiresAt
            );
        }
        catch (Exception ex)
        {
            return new LoginResponse(
                OperationId: command.OperationId,
                Success: false,
                Message: ex.Message
            );
        }
    }
}
