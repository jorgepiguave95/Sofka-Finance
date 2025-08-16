using Microsoft.AspNetCore.Mvc;
using ApiGateway.Messaging;
using SofkaFinance.Contracts.Customers;
using ApiGateway.Dtos;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMessagingClient _client;
    public AuthController(IMessagingClient client) => _client = client;

    [HttpPost("login")]
    public async Task<ActionResult<Login>> Login([FromBody] LoginDto body)
    {
        try
        {
            var operationId = Guid.NewGuid();
            var cmd = new LoginCommand(operationId, body.Email, body.Password);

            var contractResponse = await _client.RequestAsync<LoginCommand, LoginResponse>(cmd);

            if (contractResponse.Success)
            {
                var gatewayResponse = new Login(
                    Message: "Inicio de sesion exitoso",
                    Auth: new AuthEntity(
                        Correo: contractResponse.CustomerEmail ?? "",
                        Token: contractResponse.Token ?? "",
                        FechaExpiracion: contractResponse.ExpiresAt ?? DateTime.UtcNow.AddHours(24),
                        IdCliente: contractResponse.CustomerId ?? Guid.Empty
                    )
                );

                return Ok(gatewayResponse);
            }

            return BadRequest(new Response(contractResponse.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Response($"Error procesando inicio de sesion: {ex.Message}"));
        }
    }
}
