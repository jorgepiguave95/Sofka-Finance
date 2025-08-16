using Account;
using SofkaFinance.Contracts.Accounts;

namespace Account.Api.Controllers;

public class MovementsController
{
    public Task GetByAccount(GetMovementsByAccountQuery query)
    {
        Console.WriteLine($"GetByAccount query processed for AccountId: {query.AccountId}");
        return Task.CompletedTask;
        // => throw new NotImplementedException();
    }

    public Task Deposit(DepositCommand command)
    {
        Console.WriteLine($"Deposit processed: {command.Amount} to AccountId: {command.AccountId}");
        if (!string.IsNullOrEmpty(command.Concept))
            Console.WriteLine($"  Concept: {command.Concept}");
        return Task.CompletedTask;
        // => throw new NotImplementedException();
    }

    public Task Withdraw(WithdrawCommand command)
    {
        Console.WriteLine($"Withdraw processed: {command.Amount} from AccountId: {command.AccountId}");
        if (!string.IsNullOrEmpty(command.Concept))
            Console.WriteLine($"  Concept: {command.Concept}");
        return Task.CompletedTask;
        // => throw new NotImplementedException();
    }

    public Task Transfer(TransferCommand command)
    {
        Console.WriteLine($"Transfer processed: {command.Amount} from {command.FromAccountId} to {command.ToAccountId}");
        if (!string.IsNullOrEmpty(command.Concept))
            Console.WriteLine($"  Concept: {command.Concept}");
        return Task.CompletedTask;
        // => throw new NotImplementedException();
    }

    public Task GetReport(GetMovementsReportQuery query)
    {
        Console.WriteLine($"GetReport query processed for AccountId: {query.AccountId}");
        return Task.CompletedTask;
        // => throw new NotImplementedException();
    }
}
