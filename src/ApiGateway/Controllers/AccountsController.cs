using ApiGateway.Dtos;
using ApiGateway.Messaging;
using Microsoft.AspNetCore.Mvc;
using SofkaFinance.Contracts.Accounts;
using System.Text.Json;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/cuentas")]
public class AccountsController : ControllerBase
{
    private readonly IMessagingClient _client;

    public AccountsController(IMessagingClient client)
    {
        _client = client;
    }

    [HttpGet]
    public async Task<ActionResult<AccountsList>> GetAllAccounts()
    {
        try
        {
            var operationId = Guid.NewGuid();
            var query = new GetAllAccountsQuery(operationId);

            var contractResponse = await _client.RequestAsync<GetAllAccountsQuery, GetAllAccountsResponse>(query);

            if (contractResponse.Accounts != null)
            {
                var accountEntities = contractResponse.Accounts.Select(account =>
                {
                    var json = JsonSerializer.Serialize(account);
                    var accountData = JsonSerializer.Deserialize<AccountEntity>(json);

                    if (accountData == null) return null;

                    return accountData;
                }).Where(x => x != null).Cast<AccountEntity>().ToArray();

                var gatewayResponse = new AccountsList(
                    Message: "Cuentas obtenidas exitosamente",
                    Cuentas: accountEntities
                );

                return Ok(gatewayResponse);
            }

            return BadRequest(new Response(contractResponse.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Response($"Error procesando consulta de cuentas: {ex.Message}"));
        }
    }

    [HttpGet("{accountId:guid}")]
    public async Task<ActionResult<Account>> GetAccountById(Guid accountId)
    {
        try
        {
            var operationId = Guid.NewGuid();
            var query = new GetAccountByIdQuery(operationId, accountId);

            var contractResponse = await _client.RequestAsync<GetAccountByIdQuery, GetAccountResponse>(query);

            if (contractResponse.Account != null)
            {
                var json = JsonSerializer.Serialize(contractResponse.Account);
                var accountData = JsonSerializer.Deserialize<AccountEntity>(json);

                if (accountData != null)
                {
                    var gatewayResponse = new Account(
                        Message: "Cuenta obtenida exitosamente",
                        Cuenta: accountData
                    );

                    return Ok(gatewayResponse);
                }
            }

            return BadRequest(new Response(contractResponse.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Response($"Error procesando consulta de cuenta: {ex.Message}"));
        }
    }

    [HttpPost]
    public async Task<ActionResult<Account>> CreateAccount([FromBody] CreateAccountDto dto)
    {
        try
        {
            var operationId = Guid.NewGuid();
            var command = new CreateAccountCommand(operationId, dto.CustomerId, dto.AccountType);

            var contractResponse = await _client.RequestAsync<CreateAccountCommand, CreateAccountResponse>(command);

            if (contractResponse.AccountId.HasValue)
            {
                var gatewayResponse = new Account(
                    Message: "Cuenta creada exitosamente",
                    Cuenta: new AccountEntity(
                        Id: contractResponse.AccountId.Value,
                        IdCliente: dto.CustomerId,
                        NumeroCuenta: $"ACC-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}",
                        TipoCuenta: dto.AccountType,
                        Saldo: 0.00m,
                        EstaActiva: true,
                        FechaCreacion: DateTime.UtcNow
                    )
                );

                return Ok(gatewayResponse);
            }

            return BadRequest(new Response(contractResponse.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Response($"Error procesando creacion de cuenta: {ex.Message}"));
        }
    }

    [HttpDelete("{accountId:guid}")]
    public async Task<ActionResult<Response>> CloseAccount(Guid accountId)
    {
        try
        {
            var operationId = Guid.NewGuid();
            var command = new CloseAccountCommand(operationId, accountId);

            var contractResponse = await _client.RequestAsync<CloseAccountCommand, CloseAccountResponse>(command);

            if (contractResponse.AccountId.HasValue)
            {
                return Ok(new Response("Cuenta cerrada exitosamente"));
            }

            return BadRequest(new Response(contractResponse.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Response($"Error procesando cierre de cuenta: {ex.Message}"));
        }
    }
}
