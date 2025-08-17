using Microsoft.AspNetCore.Mvc;
using ApiGateway.Messaging;
using ApiGateway.Dtos;
using SofkaFinance.Contracts.Accounts;
using System.Text.Json;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/movimientos")]
public class MovementsController : ControllerBase
{
    private readonly IMessagingClient _client;
    public MovementsController(IMessagingClient client) => _client = client;

    [HttpPost("{accountId:guid}/deposito")]
    public async Task<ActionResult<Response>> Deposit([FromRoute] Guid accountId, [FromBody] DepositDto body)
    {
        try
        {
            var operationId = Guid.NewGuid();
            var cmd = new DepositCommand(operationId, accountId, body.Amount, body.Concept);

            var contractResponse = await _client.RequestAsync<DepositCommand, DepositResponse>(cmd);

            if (contractResponse.MovementId.HasValue)
            {
                return Ok(new Response("Deposito realizado exitosamente"));
            }

            return BadRequest(new Response(contractResponse.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Response($"Error procesando deposito: {ex.Message}"));
        }
    }

    [HttpPost("{accountId:guid}/retiro")]
    public async Task<ActionResult<Response>> Withdraw([FromRoute] Guid accountId, [FromBody] WithdrawDto body)
    {
        try
        {
            var operationId = Guid.NewGuid();
            var cmd = new WithdrawCommand(operationId, accountId, body.Amount, body.Concept);

            var contractResponse = await _client.RequestAsync<WithdrawCommand, WithdrawResponse>(cmd);

            if (contractResponse.MovementId.HasValue)
            {
                return Ok(new Response("Retiro realizado exitosamente"));
            }

            return BadRequest(new Response(contractResponse.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Response($"Error procesando retiro: {ex.Message}"));
        }
    }

    [HttpPost("transferencia")]
    public async Task<ActionResult<Response>> Transfer([FromBody] TransferDto body)
    {
        try
        {
            var operationId = Guid.NewGuid();
            var cmd = new TransferCommand(operationId, body.FromAccountId, body.ToAccountId, body.Amount, body.Concept);

            var contractResponse = await _client.RequestAsync<TransferCommand, TransferResponse>(cmd);

            if (contractResponse.MovementId.HasValue)
            {
                return Ok(new Response("Transferencia realizada exitosamente"));
            }

            return BadRequest(new Response(contractResponse.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Response($"Error procesando transferencia: {ex.Message}"));
        }
    }

    [HttpGet("cuenta/{accountId:guid}")]
    public async Task<ActionResult<MovementsList>> GetByAccount([FromRoute] Guid accountId)
    {
        try
        {
            var operationId = Guid.NewGuid();

            var query = new GetMovementsByAccountQuery(operationId, accountId);

            var contractResponse = await _client.RequestAsync<GetMovementsByAccountQuery, GetMovementsByAccountResponse>(query);

            if (contractResponse?.Movements != null)
            {
                var movementEntities = contractResponse.Movements.Select(movement =>
                {
                    var json = JsonSerializer.Serialize(movement);
                    var movementData = JsonSerializer.Deserialize<MovementEntity>(json);

                    if (movementData == null) return null;

                    return movementData;
                }).Where(x => x != null).Cast<MovementEntity>().ToArray();

                var gatewayResponse = new MovementsList(
                    Message: "Movimientos obtenidos exitosamente",
                    Movimientos: movementEntities
                );

                return Ok(gatewayResponse);
            }

            return BadRequest(new Response(contractResponse?.Message ?? "Error desconocido"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Response($"Error procesando consulta de movimientos: {ex.Message}"));
        }
    }

    [HttpGet("reportes")]
    public async Task<ActionResult<MovementsList>> Report(
        [FromQuery] DateTime fechaInicio,
        [FromQuery] DateTime fechaFin,
        [FromQuery] Guid idCliente)
    {
        try
        {
            var operationId = Guid.NewGuid();
            var query = new GetMovementsReportQuery(operationId, idCliente, fechaInicio, fechaFin);

            var contractResponse = await _client.RequestAsync<GetMovementsReportQuery, GetMovementsReportResponse>(query);

            if (contractResponse.Movements != null)
            {
                var movementEntities = contractResponse.Movements.Select(movement =>
                {
                    var json = JsonSerializer.Serialize(movement);
                    var movementData = JsonSerializer.Deserialize<MovementEntity>(json);

                    if (movementData == null) return null;

                    return movementData;
                }).Where(x => x != null).Cast<MovementEntity>().ToArray();

                var gatewayResponse = new MovementsList(
                    Message: "Movimientos obtenidos exitosamente",
                    Movimientos: movementEntities
                );

                return Ok(gatewayResponse);
            }

            return BadRequest(new Response(contractResponse.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Response($"Error procesando consulta de movimientos: {ex.Message}"));
        }
    }
}
