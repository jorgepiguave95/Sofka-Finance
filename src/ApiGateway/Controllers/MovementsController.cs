using Microsoft.AspNetCore.Mvc;
using ApiGateway.Messaging;
using ApiGateway.Dtos;
using SofkaFinance.Contracts.Accounts;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/movimientos")]
public class MovementsController : ControllerBase
{
    private readonly IMessagingClient _client;
    public MovementsController(IMessagingClient client) => _client = client;

    [HttpPost("{accountId:guid}/deposito")]
    public async Task<ActionResult<MovementOperationResponse>> Deposit([FromRoute] Guid accountId, [FromBody] DepositDto body)
    {
        try
        {
            var operationId = Guid.NewGuid();
            var cmd = new DepositCommand(operationId, accountId, body.Amount, body.Concept);
            await _client.SendAsync("finance.accounts", "account.deposit", cmd);

            var response = new MovementOperationResponse(
                OperationId: operationId,
                Operation: "Deposit",
                AccountId: accountId,
                Message: $"Deposito de ${body.Amount} a la cuenta '{accountId}' fue excitoso"
            );

            return Accepted(response);
        }
        catch (Exception ex)
        {
            var errorResponse = new MovementOperationResponse(
                OperationId: Guid.Empty,
                Operation: "Deposit",
                AccountId: accountId,
                Message: $"Error procesando deposito: {ex.Message}",
                StatusCode: 500
            );
            return StatusCode(500, errorResponse);
        }
    }

    [HttpPost("{accountId:guid}/retiro")]
    public async Task<ActionResult<MovementOperationResponse>> Withdraw([FromRoute] Guid accountId, [FromBody] WithdrawDto body)
    {
        try
        {
            var operationId = Guid.NewGuid();
            var cmd = new WithdrawCommand(operationId, accountId, body.Amount, body.Concept);
            await _client.SendAsync("finance.accounts", "account.withdraw", cmd);

            var response = new MovementOperationResponse(
                OperationId: operationId,
                Operation: "Withdraw",
                AccountId: accountId,
                Message: $"Retiro de ${body.Amount} de la cuenta '{accountId}' fue excitoso"
            );

            return Accepted(response);
        }
        catch (Exception ex)
        {
            var errorResponse = new MovementOperationResponse(
                OperationId: Guid.Empty,
                Operation: "Withdraw",
                AccountId: accountId,
                Message: $"Error procesando retiro: {ex.Message}",
                StatusCode: 500
            );
            return StatusCode(500, errorResponse);
        }
    }

    [HttpPost("transferencia")]
    public async Task<ActionResult<MovementOperationResponse>> Transfer([FromBody] TransferDto body)
    {
        try
        {
            var operationId = Guid.NewGuid();
            var cmd = new TransferCommand(operationId, body.FromAccountId, body.ToAccountId, body.Amount, body.Concept);
            await _client.SendAsync("finance.accounts", "account.transfer", cmd);

            var response = new MovementOperationResponse(
                OperationId: operationId,
                Operation: "Transfer",
                AccountId: body.FromAccountId,
                Message: $"Transferencia de ${body.Amount} de la cuenta '{body.FromAccountId}' a '{body.ToAccountId}' fue excitoso"
            );

            return Accepted(response);
        }
        catch (Exception ex)
        {
            var errorResponse = new MovementOperationResponse(
                OperationId: Guid.Empty,
                Operation: "Transfer",
                AccountId: body.FromAccountId,
                Message: $"Error procesando transferencia: {ex.Message}",
                StatusCode: 500
            );
            return StatusCode(500, errorResponse);
        }
    }

    [HttpGet("cuenta/{accountId:guid}")]
    public async Task<ActionResult<MovementOperationResponse>> GetByAccount([FromRoute] Guid accountId)
    {
        try
        {
            var operationId = Guid.NewGuid();
            var cmd = new GetMovementsByAccountQuery(operationId, accountId);
            await _client.SendAsync("finance.accounts", "movement.getByAccount", cmd);

            var response = new MovementOperationResponse(
                OperationId: operationId,
                Operation: "GetMovementsByAccount",
                AccountId: accountId,
                Message: $"Solicitud para obtener movimientos de la cuenta '{accountId}' fue exitosa"
            );

            return Accepted(response);
        }
        catch (Exception ex)
        {
            var errorResponse = new MovementOperationResponse(
                OperationId: Guid.Empty,
                Operation: "GetMovementsByAccount",
                AccountId: accountId,
                Message: $"Error procesando consulta de movimientos: {ex.Message}",
                StatusCode: 500
            );
            return StatusCode(500, errorResponse);
        }
    }

    [HttpGet("reportes")]
    public async Task<ActionResult<MovementOperationResponse>> Report(
        [FromQuery] DateTime fechaInicio,
        [FromQuery] DateTime fechaFin,
        [FromBody] GetMovementReportDto body)
    {
        try
        {
            var operationId = Guid.NewGuid();
            var cmd = new GetMovementsReportQuery(operationId, body.IdCliente, fechaInicio, fechaFin);
            await _client.SendAsync("finance.accounts", "movement.getReport", cmd);

            var response = new MovementOperationResponse(
                OperationId: operationId,
                Operation: "GetMovementsReport",
                Message: $"Solicitud de reporte de movimientos para el cliente '{body.IdCliente}' desde {fechaInicio:yyyy-MM-dd} hasta {fechaFin:yyyy-MM-dd} fue excitoso"
            );

            return Accepted(response);
        }
        catch (Exception ex)
        {
            var errorResponse = new MovementOperationResponse(
                OperationId: Guid.Empty,
                Operation: "GetMovementsReport",
                Message: $"Error procesando reporte de movimientos: {ex.Message}",
                StatusCode: 500
            );
            return StatusCode(500, errorResponse);
        }
    }
}
