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
                    using var document = JsonDocument.Parse(json);
                    var root = document.RootElement;

                    return new MovementEntity(
                        Id: root.TryGetProperty("MovementId", out var movementId) ? movementId.GetGuid() : Guid.Empty,
                        IdCuenta: root.TryGetProperty("AccountId", out var accountId) ? accountId.GetGuid() : Guid.Empty,
                        Tipo: root.TryGetProperty("Type", out var type) ? type.GetString() ?? "" : "",
                        Monto: root.TryGetProperty("Amount", out var amount) ? amount.GetDecimal() : 0,
                        Concepto: root.TryGetProperty("Concept", out var concept) ? concept.GetString() ?? "" : "",
                        SaldoAnterior: root.TryGetProperty("PreviousBalance", out var prevBalance) ? prevBalance.GetDecimal() : 0,
                        SaldoNuevo: root.TryGetProperty("NewBalance", out var newBalance) ? newBalance.GetDecimal() : 0,
                        FechaCreacion: root.TryGetProperty("CreatedAt", out var createdAt) ? createdAt.GetDateTime() : DateTime.MinValue
                    );
                }).ToArray();

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

    [HttpPost("reportes")]
    public async Task<ActionResult<MovementsList>> Report([FromBody] ReportRequest request)
    {
        try
        {
            var getAccountsOperationId = Guid.NewGuid();
            var getAccountsQuery = new GetAccountsByCustomerQuery(getAccountsOperationId, request.IdCliente);

            var accountsResponse = await _client.RequestAsync<GetAccountsByCustomerQuery, GetAccountsByCustomerResponse>(getAccountsQuery);

            if (accountsResponse?.Accounts == null || !accountsResponse.Accounts.Any())
            {
                return Ok(new MovementsList(
                    Message: "No se encontraron cuentas para el cliente",
                    Movimientos: Array.Empty<MovementEntity>()
                ));
            }

            var allMovements = new List<MovementEntity>();

            foreach (var account in accountsResponse.Accounts)
            {
                var movementsOperationId = Guid.NewGuid();
                var accountJson = JsonSerializer.Serialize(account);
                using var document = JsonDocument.Parse(accountJson);
                var root = document.RootElement;

                var accountId = root.TryGetProperty("AccountId", out var idProp) ? idProp.GetGuid() : Guid.Empty;

                if (accountId != Guid.Empty)
                {
                    var movementsQuery = new GetMovementsByAccountQuery(movementsOperationId, accountId, request.FechaInicio, request.FechaFin);

                    var movementsResponse = await _client.RequestAsync<GetMovementsByAccountQuery, GetMovementsByAccountResponse>(movementsQuery);

                    if (movementsResponse?.Movements != null)
                    {
                        var movementEntities = movementsResponse.Movements.Select(movement =>
                        {
                            var json = JsonSerializer.Serialize(movement);
                            using var doc = JsonDocument.Parse(json);
                            var movRoot = doc.RootElement;

                            return new MovementEntity(
                                Id: movRoot.TryGetProperty("MovementId", out var movementId) ? movementId.GetGuid() : Guid.Empty,
                                IdCuenta: movRoot.TryGetProperty("AccountId", out var accountIdProp) ? accountIdProp.GetGuid() : Guid.Empty,
                                Tipo: movRoot.TryGetProperty("Type", out var type) ? type.GetString() ?? "" : "",
                                Monto: movRoot.TryGetProperty("Amount", out var amount) ? amount.GetDecimal() : 0,
                                Concepto: movRoot.TryGetProperty("Concept", out var concept) ? concept.GetString() ?? "" : "",
                                SaldoAnterior: movRoot.TryGetProperty("PreviousBalance", out var prevBalance) ? prevBalance.GetDecimal() : 0,
                                SaldoNuevo: movRoot.TryGetProperty("NewBalance", out var newBalance) ? newBalance.GetDecimal() : 0,
                                FechaCreacion: movRoot.TryGetProperty("CreatedAt", out var createdAt) ? createdAt.GetDateTime() : DateTime.MinValue
                            );
                        });

                        allMovements.AddRange(movementEntities);
                    }
                }
            }

            var gatewayResponse = new MovementsList(
                Message: $"Reporte generado exitosamente para el cliente {request.IdCliente}. Período: {request.FechaInicio:yyyy-MM-dd} al {request.FechaFin:yyyy-MM-dd}",
                Movimientos: allMovements.OrderByDescending(m => m.FechaCreacion).ToArray()
            );

            return Ok(gatewayResponse);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Response($"Error procesando reporte: {ex.Message}"));
        }
    }
}
