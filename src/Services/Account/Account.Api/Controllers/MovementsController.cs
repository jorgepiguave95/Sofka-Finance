using Account;
using SofkaFinance.Contracts.Accounts;

namespace Account.Api.Controllers;

public class MovementsController
{
    public async Task<GetMovementsByAccountResponse> GetByAccount(GetMovementsByAccountQuery query)
    {
        try
        {
            var movements = new[]
            {
                new
                {
                    MovementId = Guid.NewGuid(),
                    AccountId = query.AccountId,
                    Type = "Deposit",
                    Amount = 1000.00m,
                    Concept = "Initial deposit",
                    PreviousBalance = 0.00m,
                    NewBalance = 1000.00m,
                    CreatedAt = DateTime.UtcNow.AddDays(-10)
                },
                new
                {
                    MovementId = Guid.NewGuid(),
                    AccountId = query.AccountId,
                    Type = "Withdraw",
                    Amount = 200.00m,
                    Concept = "ATM withdrawal",
                    PreviousBalance = 1000.00m,
                    NewBalance = 800.00m,
                    CreatedAt = DateTime.UtcNow.AddDays(-5)
                }
            };

            await Task.CompletedTask;

            return new GetMovementsByAccountResponse(
                OperationId: query.OperationId,
                Success: true,
                Message: "Movements retrieved successfully",
                Movements: movements.Cast<object>().ToArray()
            );
        }
        catch (Exception ex)
        {
            return new GetMovementsByAccountResponse(
                OperationId: query.OperationId,
                Success: false,
                Message: ex.Message,
                Movements: null
            );
        }
    }

    public async Task<DepositResponse> Deposit(DepositCommand command)
    {
        try
        {
            var movementId = Guid.NewGuid();
            var newBalance = 1000.00m + command.Amount;

            await Task.CompletedTask;

            return new DepositResponse(
                OperationId: command.OperationId,
                Success: true,
                Message: "Deposit completed successfully",
                MovementId: movementId,
                NewBalance: newBalance
            );
        }
        catch (Exception ex)
        {
            return new DepositResponse(
                OperationId: command.OperationId,
                Success: false,
                Message: ex.Message
            );
        }
    }

    public async Task<WithdrawResponse> Withdraw(WithdrawCommand command)
    {
        try
        {
            var currentBalance = 1000.00m;

            if (currentBalance < command.Amount)
            {
                return new WithdrawResponse(
                    OperationId: command.OperationId,
                    Success: false,
                    Message: "Insufficient funds"
                );
            }

            var movementId = Guid.NewGuid();
            var newBalance = currentBalance - command.Amount;

            await Task.CompletedTask;

            return new WithdrawResponse(
                OperationId: command.OperationId,
                Success: true,
                Message: "Withdrawal completed successfully",
                MovementId: movementId,
                NewBalance: newBalance
            );
        }
        catch (Exception ex)
        {
            return new WithdrawResponse(
                OperationId: command.OperationId,
                Success: false,
                Message: ex.Message
            );
        }
    }

    public async Task<TransferResponse> Transfer(TransferCommand command)
    {
        try
        {
            var fromAccountBalance = 1000.00m;
            var toAccountBalance = 500.00m;

            if (fromAccountBalance < command.Amount)
            {
                return new TransferResponse(
                    OperationId: command.OperationId,
                    Success: false,
                    Message: "Insufficient funds in source account"
                );
            }

            var movementId = Guid.NewGuid();
            var fromAccountNewBalance = fromAccountBalance - command.Amount;
            var toAccountNewBalance = toAccountBalance + command.Amount;

            await Task.CompletedTask;

            return new TransferResponse(
                OperationId: command.OperationId,
                Success: true,
                Message: "Transfer completed successfully",
                MovementId: movementId,
                FromAccountNewBalance: fromAccountNewBalance,
                ToAccountNewBalance: toAccountNewBalance
            );
        }
        catch (Exception ex)
        {
            return new TransferResponse(
                OperationId: command.OperationId,
                Success: false,
                Message: ex.Message
            );
        }
    }

    public async Task<GetMovementsReportResponse> GetReport(GetMovementsReportQuery query)
    {
        try
        {
            var movements = new[]
            {
                new
                {
                    MovementId = Guid.NewGuid(),
                    AccountId = query.AccountId,
                    Type = "Deposit",
                    Amount = 1000.00m,
                    Concept = "Salary deposit",
                    Date = query.StartDate.AddDays(1)
                },
                new
                {
                    MovementId = Guid.NewGuid(),
                    AccountId = query.AccountId,
                    Type = "Withdraw",
                    Amount = 200.00m,
                    Concept = "ATM withdrawal",
                    Date = query.StartDate.AddDays(5)
                }
            };

            var report = new
            {
                AccountId = query.AccountId,
                StartDate = query.StartDate,
                EndDate = query.EndDate,
                TotalDeposits = movements.Where(m => m.Type == "Deposit").Sum(m => m.Amount),
                TotalWithdrawals = movements.Where(m => m.Type == "Withdraw").Sum(m => m.Amount),
                NetAmount = movements.Where(m => m.Type == "Deposit").Sum(m => m.Amount) -
                           movements.Where(m => m.Type == "Withdraw").Sum(m => m.Amount),
                MovementCount = movements.Length
            };

            await Task.CompletedTask;

            return new GetMovementsReportResponse(
                OperationId: query.OperationId,
                Success: true,
                Message: "Movement report generated successfully",
                Report: report,
                Movements: movements.Cast<object>().ToArray()
            );
        }
        catch (Exception ex)
        {
            return new GetMovementsReportResponse(
                OperationId: query.OperationId,
                Success: false,
                Message: ex.Message
            );
        }
    }
}
