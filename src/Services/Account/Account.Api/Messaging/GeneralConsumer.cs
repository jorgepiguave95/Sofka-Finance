using Account;
using MassTransit;
using Account.Api.Controllers;
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
    private readonly AccountsController _accountsController;
    private readonly MovementsController _movementsController;

    public GeneralConsumer(
        AccountsController accountsController,
        MovementsController movementsController)
    {
        _accountsController = accountsController;
        _movementsController = movementsController;

    }

    // Account Commands
    public async Task Consume(ConsumeContext<CreateAccountCommand> context)
    {
        try
        {
            Console.WriteLine($"*** RECEIVED CreateAccountCommand: {context.Message.OperationId} ***");
            Console.WriteLine($"CustomerId: {context.Message.CustomerId}, AccountType: {context.Message.AccountType}");

            var result = await _accountsController.Create(context.Message);

            Console.WriteLine($"*** SENDING CreateAccount Response: {result.Success} ***");
            await context.RespondAsync(result);
            Console.WriteLine($"*** RESPONSE SENT ***");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"*** ERROR in CreateAccount: {ex.Message} ***");
            await context.RespondAsync(new CreateAccountResponse(
                OperationId: context.Message.OperationId,
                Success: false,
                Message: ex.Message,
                AccountId: null
            ));
        }
    }

    public async Task Consume(ConsumeContext<CloseAccountCommand> context)
    {
        try
        {
            Console.WriteLine($"Processing CloseAccountCommand: {context.Message.OperationId}");
            var result = await _accountsController.Close(context.Message);
            await context.RespondAsync(result);
        }
        catch (Exception ex)
        {
            await context.RespondAsync(new CloseAccountResponse(
                OperationId: context.Message.OperationId,
                Success: false,
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
            Console.WriteLine($"Processing DepositCommand: {context.Message.OperationId}");
            var result = await _movementsController.Deposit(context.Message);
            await context.RespondAsync(result);
        }
        catch (Exception ex)
        {
            await context.RespondAsync(new DepositResponse(
                OperationId: context.Message.OperationId,
                Success: false,
                Message: ex.Message,
                MovementId: null
            ));
        }
    }

    public async Task Consume(ConsumeContext<WithdrawCommand> context)
    {
        try
        {
            Console.WriteLine($"Processing WithdrawCommand: {context.Message.OperationId}");
            var result = await _movementsController.Withdraw(context.Message);
            await context.RespondAsync(result);
        }
        catch (Exception ex)
        {
            await context.RespondAsync(new WithdrawResponse(
                OperationId: context.Message.OperationId,
                Success: false,
                Message: ex.Message,
                MovementId: null
            ));
        }
    }

    public async Task Consume(ConsumeContext<TransferCommand> context)
    {
        try
        {
            Console.WriteLine($"Processing TransferCommand: {context.Message.OperationId}");
            var result = await _movementsController.Transfer(context.Message);
            await context.RespondAsync(result);
        }
        catch (Exception ex)
        {
            await context.RespondAsync(new TransferResponse(
                OperationId: context.Message.OperationId,
                Success: false,
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
            Console.WriteLine($"Processing GetAccountByIdQuery");
            var result = await _accountsController.GetById(context.Message);
            await context.RespondAsync(result);
        }
        catch (Exception ex)
        {
            await context.RespondAsync(new GetAccountResponse(
                OperationId: context.Message.OperationId,
                Success: false,
                Message: ex.Message,
                Account: null
            ));
        }
    }

    public async Task Consume(ConsumeContext<GetAccountsByCustomerQuery> context)
    {
        try
        {
            Console.WriteLine($"Processing GetAccountsByCustomerQuery");
            var result = await _accountsController.GetByCustomer(context.Message);
            await context.RespondAsync(result);
        }
        catch (Exception ex)
        {
            await context.RespondAsync(new GetAccountsByCustomerResponse(
                OperationId: context.Message.OperationId,
                Success: false,
                Message: ex.Message,
                Accounts: null
            ));
        }
    }

    public async Task Consume(ConsumeContext<GetAllAccountsQuery> context)
    {
        try
        {
            Console.WriteLine($"Processing GetAllAccountsQuery");
            var result = await _accountsController.GetAll(context.Message);
            await context.RespondAsync(result);
        }
        catch (Exception ex)
        {
            await context.RespondAsync(new GetAllAccountsResponse(
                OperationId: context.Message.OperationId,
                Success: false,
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
            Console.WriteLine($"Processing GetMovementsByAccountQuery");
            var result = await _movementsController.GetByAccount(context.Message);
            await context.RespondAsync(result);
        }
        catch (Exception ex)
        {
            await context.RespondAsync(new GetMovementsByAccountResponse(
                OperationId: context.Message.OperationId,
                Success: false,
                Message: ex.Message,
                Movements: null
            ));
        }
    }

    public async Task Consume(ConsumeContext<GetMovementsReportQuery> context)
    {
        try
        {
            Console.WriteLine($"Processing GetMovementsReportQuery");
            var result = await _movementsController.GetReport(context.Message);
            await context.RespondAsync(result);
        }
        catch (Exception ex)
        {
            await context.RespondAsync(new GetMovementsReportResponse(
                OperationId: context.Message.OperationId,
                Success: false,
                Message: ex.Message,
                Report: null
            ));
        }
    }
}
