using ApiGateway.Dtos;
using ApiGateway.Messaging;
using Microsoft.AspNetCore.Mvc;
using SofkaFinance.Contracts.Accounts;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/cuentas")]
public class AccountsController : ControllerBase
{
    private readonly IMessagingClient _messagingClient;

    public AccountsController(IMessagingClient messagingClient)
    {
        _messagingClient = messagingClient;
    }

    [HttpGet]
    public async Task<ActionResult<AccountOperationResponse>> GetAllAccounts()
    {
        try
        {
            var operationId = Guid.NewGuid();
            var query = new GetAllAccountsQuery(operationId);

            await _messagingClient.SendAsync("finance.accounts", "account.getAll", query);

            return Accepted(new AccountOperationResponse(
                OperationId: operationId,
                Operation: "GetAllAccounts",
                Message: "Solicitud para obtener todas las cuentas fue excitosa"
            ));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new AccountOperationResponse(
                OperationId: Guid.NewGuid(),
                Operation: "GetAllAccounts",
                Message: $"Error processing accounts query: {ex.Message}",
                StatusCode: 500
            ));
        }
    }

    [HttpGet("{accountId:guid}")]
    public async Task<ActionResult<AccountOperationResponse>> GetAccountById(Guid accountId)
    {
        try
        {
            var operationId = Guid.NewGuid();
            var query = new GetAccountByIdQuery(operationId, accountId);

            await _messagingClient.SendAsync("finance.accounts", "account.getById", query);

            return Accepted(new AccountOperationResponse(
                OperationId: operationId,
                Operation: "GetAccountById",
                Message: $"Solicitud para obtener la cuenta '{accountId}' fue exitosa"
            ));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new AccountOperationResponse(
                OperationId: Guid.NewGuid(),
                Operation: "GetAccountById",
                Message: $"Error procesando consulta de cuenta: {ex.Message}",
                StatusCode: 500
            ));
        }
    }

    [HttpPost]
    public async Task<ActionResult<AccountOperationResponse>> CreateAccount([FromBody] CreateAccountDto dto)
    {
        try
        {
            var operationId = Guid.NewGuid();
            var command = new CreateAccountCommand(operationId, dto.CustomerId, dto.AccountType);

            await _messagingClient.SendAsync("finance.accounts", "account.create", command);

            return Accepted(new AccountOperationResponse(
                OperationId: operationId,
                Operation: "CreateAccount",
                Message: "Solicitud de creacion de cuenta fue exitosa"
            ));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new AccountOperationResponse(
                OperationId: Guid.NewGuid(),
                Operation: "CreateAccount",
                Message: $"Error procesando creacion de cuenta: {ex.Message}",
                StatusCode: 500
            ));
        }
    }

    [HttpDelete("{accountId:guid}")]
    public async Task<ActionResult<AccountOperationResponse>> CloseAccount(Guid accountId)
    {
        try
        {
            var operationId = Guid.NewGuid();
            var command = new CloseAccountCommand(operationId, accountId);

            await _messagingClient.SendAsync("finance.accounts", "account.close", command);

            return Accepted(new AccountOperationResponse(
                OperationId: operationId,
                Operation: "CloseAccount",
                Message: $"Solicitud de cierre de cuenta '{accountId}' fue exitosa"
            ));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new AccountOperationResponse(
                OperationId: Guid.NewGuid(),
                Operation: "CloseAccount",
                Message: $"Error procesando cierre de cuenta: {ex.Message}",
                StatusCode: 500
            ));
        }
    }
}
