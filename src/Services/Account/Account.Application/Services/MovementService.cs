using Account.Application.Repositories;
using SofkaFinance.Contracts.Accounts;
using Account.Application.Interfaces;

namespace Account.Application.Services;

public class MovementService : IMovementService
{
    private readonly IMovementRepository _movementRepository;
    private readonly IAccountRepository _accountRepository;

    public MovementService(IMovementRepository movementRepository, IAccountRepository accountRepository)
    {
        _movementRepository = movementRepository;
        _accountRepository = accountRepository;
    }

    public async Task<GetMovementsByAccountResponse> GetByAccountAsync(GetMovementsByAccountQuery query)
    {
        try
        {
            var movements = await _movementRepository.GetByAccountIdAsync(query.AccountId);

            var movementData = movements.Select(m => new
            {
                MovementId = m.Id,
                AccountId = m.AccountId,
                Type = m.Type.Value,
                Amount = m.Value,
                Concept = m.Concept,
                PreviousBalance = m.AvailableBalance - m.Value,
                NewBalance = m.AvailableBalance,
                CreatedAt = m.Date
            }).ToArray();

            return new GetMovementsByAccountResponse(
                OperationId: query.OperationId,
                Message: "Movimientos obtenidos exitosamente",
                Movements: movementData.Cast<object>().ToArray()
            );
        }
        catch (Exception ex)
        {
            return new GetMovementsByAccountResponse(
                OperationId: query.OperationId,
                Message: ex.Message,
                Movements: null
            );
        }
    }

    public async Task<DepositResponse> DepositAsync(DepositCommand command)
    {
        try
        {
            var account = await _accountRepository.GetByIdAsync(command.AccountId);
            if (account == null)
            {
                return new DepositResponse(
                    OperationId: command.OperationId,
                    Message: "Cuenta no encontrada"
                );
            }

            var movement = account.Deposit(command.Amount, command.Concept);

            await _movementRepository.CreateAsync(movement);
            await _accountRepository.UpdateAsync(account);

            return new DepositResponse(
                OperationId: command.OperationId,
                Message: "Depósito realizado exitosamente",
                MovementId: movement.Id,
                NewBalance: account.Balance
            );
        }
        catch (Exception ex)
        {
            return new DepositResponse(
                OperationId: command.OperationId,
                Message: ex.Message
            );
        }
    }

    public async Task<WithdrawResponse> WithdrawAsync(WithdrawCommand command)
    {
        try
        {
            var account = await _accountRepository.GetByIdAsync(command.AccountId);
            if (account == null)
            {
                return new WithdrawResponse(
                    OperationId: command.OperationId,
                    Message: "Cuenta no encontrada"
                );
            }

            var movement = account.Withdraw(command.Amount, command.Concept);

            await _movementRepository.CreateAsync(movement);
            await _accountRepository.UpdateAsync(account);

            return new WithdrawResponse(
                OperationId: command.OperationId,
                Message: "Retiro realizado exitosamente",
                MovementId: movement.Id,
                NewBalance: account.Balance
            );
        }
        catch (Exception ex)
        {
            return new WithdrawResponse(
                OperationId: command.OperationId,
                Message: ex.Message
            );
        }
    }

    public async Task<TransferResponse> TransferAsync(TransferCommand command)
    {
        try
        {
            var fromAccount = await _accountRepository.GetByIdAsync(command.FromAccountId);
            var toAccount = await _accountRepository.GetByIdAsync(command.ToAccountId);

            if (fromAccount == null)
            {
                return new TransferResponse(
                    OperationId: command.OperationId,
                    Message: "Cuenta de origen no encontrada"
                );
            }

            if (toAccount == null)
            {
                return new TransferResponse(
                    OperationId: command.OperationId,
                    Message: "Cuenta de destino no encontrada"
                );
            }

            var (debitMovement, creditMovement) = fromAccount.Transfer(
                command.Amount,
                toAccount,
                command.Concept
            );

            await _movementRepository.CreateAsync(debitMovement);
            await _movementRepository.CreateAsync(creditMovement);
            await _accountRepository.UpdateAsync(fromAccount);
            await _accountRepository.UpdateAsync(toAccount);

            return new TransferResponse(
                OperationId: command.OperationId,
                Message: "Transferencia realizada exitosamente",
                MovementId: debitMovement.Id,
                FromAccountNewBalance: fromAccount.Balance,
                ToAccountNewBalance: toAccount.Balance
            );
        }
        catch (Exception ex)
        {
            return new TransferResponse(
                OperationId: command.OperationId,
                Message: ex.Message
            );
        }
    }

    public async Task<GetMovementsReportResponse> GetReportAsync(GetMovementsReportQuery query)
    {
        try
        {
            var movements = await _movementRepository.GetByAccountIdAndDateRangeAsync(
                query.AccountId,
                query.StartDate,
                query.EndDate
            );

            var movementData = movements.Select(m => new
            {
                MovementId = m.Id,
                AccountId = m.AccountId,
                Type = m.Type.Value,
                Amount = m.Value,
                Concept = m.Concept,
                Date = m.Date
            }).ToArray();

            var report = new
            {
                AccountId = query.AccountId,
                StartDate = query.StartDate,
                EndDate = query.EndDate,
                TotalDeposits = movements
                    .Where(m => m.Type.Value.Contains("Depósito"))
                    .Sum(m => m.Value),
                TotalWithdrawals = movements
                    .Where(m => m.Type.Value.Contains("Retiro") || m.Type.Value.Contains("Transferencia"))
                    .Sum(m => m.Value),
                NetAmount = movements
                    .Where(m => m.Type.Value.Contains("Depósito"))
                    .Sum(m => m.Value) -
                    movements
                    .Where(m => m.Type.Value.Contains("Retiro") || m.Type.Value.Contains("Transferencia"))
                    .Sum(m => m.Value),
                MovementCount = movements.Count()
            };

            return new GetMovementsReportResponse(
                OperationId: query.OperationId,
                Message: "Reporte de movimientos generado exitosamente",
                Report: report,
                Movements: movementData.Cast<object>().ToArray()
            );
        }
        catch (Exception ex)
        {
            return new GetMovementsReportResponse(
                OperationId: query.OperationId,
                Message: ex.Message
            );
        }
    }
}
