using Account;
using MassTransit;
using Account.Application.Interfaces;
using SofkaFinance.Contracts.Accounts;

namespace Account.Api.Messaging;

public class GeneralConsumer :
    IConsumer<CreateAccountCommand>,
    IConsumer<CloseAccountCommand>,
    IConsumer<DepositCommand>,
    IConsumer<WithdrawCommand>,
    IConsumer<TransferCommand>,
    IConsumer<GetAccountByIdQuery>,
    IConsumer<GetAccountsByCustomerQuery>,
    IConsumer<GetAllAccountsQuery>,
    IConsumer<GetMovementsByAccountQuery>,
    IConsumer<GetMovementsReportQuery>
{
    private readonly IAccountService _accountService;
    private readonly IMovementService _movementService;

    public GeneralConsumer(
        IAccountService accountService,
        IMovementService movementService)
    {
        _accountService = accountService;
        _movementService = movementService;
    }

    // Account Commands
    public async Task Consume(ConsumeContext<CreateAccountCommand> context)
    {
        try
        {
            var result = await _accountService.CreateAsync(context.Message);

            await context.RespondAsync(result);
        }
        catch (Exception ex)
        {
            await context.RespondAsync(new CreateAccountResponse(
                OperationId: context.Message.OperationId,
                Message: ex.Message,
                AccountId: null
            ));
        }
    }

    public async Task Consume(ConsumeContext<CloseAccountCommand> context)
    {
        try
        {
            var result = await _accountService.CloseAsync(context.Message);
            await context.RespondAsync(result);
        }
        catch (Exception ex)
        {
            await context.RespondAsync(new CloseAccountResponse(
                OperationId: context.Message.OperationId,
                Message: ex.Message,
                AccountId: context.Message.AccountId
            ));
        }
    }

    // Movement Commands
    public async Task Consume(ConsumeContext<DepositCommand> context)
    {
        try
        {
            var result = await _movementService.DepositAsync(context.Message);
            await context.RespondAsync(result);
        }
        catch (Exception ex)
        {
            await context.RespondAsync(new DepositResponse(
                OperationId: context.Message.OperationId,
                Message: ex.Message,
                MovementId: null
            ));
        }
    }

    public async Task Consume(ConsumeContext<WithdrawCommand> context)
    {
        try
        {
            var result = await _movementService.WithdrawAsync(context.Message);
            await context.RespondAsync(result);
        }
        catch (Exception ex)
        {
            await context.RespondAsync(new WithdrawResponse(
                OperationId: context.Message.OperationId,
                Message: ex.Message,
                MovementId: null
            ));
        }
    }

    public async Task Consume(ConsumeContext<TransferCommand> context)
    {
        try
        {
            var result = await _movementService.TransferAsync(context.Message);
            await context.RespondAsync(result);
        }
        catch (Exception ex)
        {
            await context.RespondAsync(new TransferResponse(
                OperationId: context.Message.OperationId,
                Message: ex.Message,
                MovementId: null
            ));
        }
    }

    // Account Queries
    public async Task Consume(ConsumeContext<GetAccountByIdQuery> context)
    {
        try
        {
            var result = await _accountService.GetByIdAsync(context.Message);
            await context.RespondAsync(result);
        }
        catch (Exception ex)
        {
            await context.RespondAsync(new GetAccountResponse(
                OperationId: context.Message.OperationId,
                Message: ex.Message,
                Account: null
            ));
        }
    }

    public async Task Consume(ConsumeContext<GetAccountsByCustomerQuery> context)
    {
        try
        {
            var result = await _accountService.GetByCustomerAsync(context.Message);
            await context.RespondAsync(result);
        }
        catch (Exception ex)
        {
            await context.RespondAsync(new GetAccountsByCustomerResponse(
                OperationId: context.Message.OperationId,
                Message: ex.Message,
                Accounts: null
            ));
        }
    }

    public async Task Consume(ConsumeContext<GetAllAccountsQuery> context)
    {
        try
        {
            var result = await _accountService.GetAllAsync(context.Message);
            await context.RespondAsync(result);
        }
        catch (Exception ex)
        {
            await context.RespondAsync(new GetAllAccountsResponse(
                OperationId: context.Message.OperationId,
                Message: ex.Message,
                Accounts: null
            ));
        }
    }

    // Movement Queries
    public async Task Consume(ConsumeContext<GetMovementsByAccountQuery> context)
    {
        try
        {
            var result = await _movementService.GetByAccountAsync(context.Message);
            await context.RespondAsync(result);
        }
        catch (Exception ex)
        {
            await context.RespondAsync(new GetMovementsByAccountResponse(
                OperationId: context.Message.OperationId,
                Message: ex.Message,
                Movements: null
            ));
        }
    }

    public async Task Consume(ConsumeContext<GetMovementsReportQuery> context)
    {
        try
        {
            var result = await _movementService.GetReportAsync(context.Message);
            await context.RespondAsync(result);
        }
        catch (Exception ex)
        {
            await context.RespondAsync(new GetMovementsReportResponse(
                OperationId: context.Message.OperationId,
                Message: ex.Message,
                Report: null
            ));
        }
    }
}
