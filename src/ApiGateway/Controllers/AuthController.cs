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
    public async Task<ActionResult<AuthOperationResponse>> Login([FromBody] LoginDto body)
    {
        try
        {
            var operationId = Guid.NewGuid();
            var cmd = new LoginCommand(operationId, body.Email, body.Password);
            await _client.SendAsync("finance.customers", "auth.login", cmd);

            var response = new AuthOperationResponse(
                OperationId: operationId,
                Operation: "Login",
                Message: $"Solicitud de inicio de sesion para usuario '{body.Email}' fue excitosa"
            );

            return Accepted(response);
        }
        catch (Exception ex)
        {
            var errorResponse = new AuthOperationResponse(
                OperationId: Guid.Empty,
                Operation: "Login",
                Message: $"Error procesando inicio de sesion: {ex.Message}",
                StatusCode: 500
            );
            return StatusCode(500, errorResponse);
        }
    }
}
