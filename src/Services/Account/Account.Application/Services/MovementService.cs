using Account.Application.Repositories;
using SofkaFinance.Contracts.Accounts;
using Account.Application.Interfaces;
using Account.Domain.Errors;

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
            if (query.AccountId == Guid.Empty)
                throw new DomainException(ErrorCodes.ACCOUNT_ID_REQUIRED, "El ID de la cuenta es requerido");

            IEnumerable<Domain.Entities.Movement> movements;

            if (query.StartDate.HasValue && query.EndDate.HasValue)
            {
                movements = await _movementRepository.GetByAccountIdAndDateRangeAsync(
                    query.AccountId,
                    query.StartDate.Value,
                    query.EndDate.Value);
            }
            else
            {
                movements = await _movementRepository.GetByAccountIdAsync(query.AccountId);
            }

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
        catch (DomainException ex)
        {
            return new GetMovementsByAccountResponse(
                OperationId: query.OperationId,
                Message: $"Error de negocio: {ex.Message}",
                Movements: null
            );
        }
        catch (Exception ex)
        {
            return new GetMovementsByAccountResponse(
                OperationId: query.OperationId,
                Message: $"Error interno del servidor: {ex.Message}",
                Movements: null
            );
        }
    }

    public async Task<DepositResponse> DepositAsync(DepositCommand command)
    {
        try
        {
            if (command.AccountId == Guid.Empty)
                throw new DomainException(ErrorCodes.ACCOUNT_ID_REQUIRED, "El ID de la cuenta es requerido");

            if (command.Amount <= 0)
                throw new DomainException(ErrorCodes.AMOUNT_INVALID, "El monto del depósito debe ser mayor a cero");

            var concept = string.IsNullOrWhiteSpace(command.Concept) ? "Depósito" : command.Concept;

            var account = await _accountRepository.GetByIdAsync(command.AccountId);
            if (account == null)
                throw new DomainException(ErrorCodes.ACCOUNT_NOT_FOUND, "Cuenta no encontrada");

            if (!account.IsActive)
                throw new DomainException(ErrorCodes.ACCOUNT_INACTIVE, "No se pueden realizar operaciones en una cuenta inactiva");

            var movement = account.Deposit(command.Amount, concept);

            await _movementRepository.CreateAsync(movement);
            await _accountRepository.UpdateAsync(account);

            return new DepositResponse(
                OperationId: command.OperationId,
                Message: "Depósito realizado exitosamente",
                MovementId: movement.Id,
                NewBalance: account.Balance
            );
        }
        catch (DomainException ex)
        {
            return new DepositResponse(
                OperationId: command.OperationId,
                Message: $"Error de negocio: {ex.Message}"
            );
        }
        catch (Exception ex)
        {
            return new DepositResponse(
                OperationId: command.OperationId,
                Message: $"Error interno del servidor: {ex.Message}"
            );
        }
    }

    public async Task<WithdrawResponse> WithdrawAsync(WithdrawCommand command)
    {
        try
        {
            if (command.AccountId == Guid.Empty)
                throw new DomainException(ErrorCodes.ACCOUNT_ID_REQUIRED, "El ID de la cuenta es requerido");

            if (command.Amount <= 0)
                throw new DomainException(ErrorCodes.AMOUNT_INVALID, "El monto del retiro debe ser mayor a cero");

            var concept = string.IsNullOrWhiteSpace(command.Concept) ? "Retiro" : command.Concept;

            var account = await _accountRepository.GetByIdAsync(command.AccountId);
            if (account == null)
                throw new DomainException(ErrorCodes.ACCOUNT_NOT_FOUND, "Cuenta no encontrada");

            if (!account.IsActive)
                throw new DomainException(ErrorCodes.ACCOUNT_INACTIVE, "No se pueden realizar operaciones en una cuenta inactiva");

            if (account.Balance < command.Amount)
                throw new DomainException(ErrorCodes.INSUFFICIENT_BALANCE, "Saldo insuficiente para realizar el retiro");

            var movement = account.Withdraw(command.Amount, concept);

            await _movementRepository.CreateAsync(movement);
            await _accountRepository.UpdateAsync(account);

            return new WithdrawResponse(
                OperationId: command.OperationId,
                Message: "Retiro realizado exitosamente",
                MovementId: movement.Id,
                NewBalance: account.Balance
            );
        }
        catch (DomainException ex)
        {
            return new WithdrawResponse(
                OperationId: command.OperationId,
                Message: $"Error de negocio: {ex.Message}"
            );
        }
        catch (Exception ex)
        {
            return new WithdrawResponse(
                OperationId: command.OperationId,
                Message: $"Error interno del servidor: {ex.Message}"
            );
        }
    }

    public async Task<TransferResponse> TransferAsync(TransferCommand command)
    {
        try
        {
            if (command.FromAccountId == Guid.Empty)
                throw new DomainException(ErrorCodes.ACCOUNT_ID_REQUIRED, "El ID de la cuenta de origen es requerido");

            if (command.ToAccountId == Guid.Empty)
                throw new DomainException(ErrorCodes.TARGET_ACCOUNT_REQUIRED, "El ID de la cuenta de destino es requerido");

            if (command.FromAccountId == command.ToAccountId)
                throw new DomainException(ErrorCodes.SAME_ACCOUNT_TRANSFER, "No se puede transferir a la misma cuenta");

            if (command.Amount <= 0)
                throw new DomainException(ErrorCodes.AMOUNT_INVALID, "El monto de la transferencia debe ser mayor a cero");

            var concept = string.IsNullOrWhiteSpace(command.Concept) ? "Transferencia" : command.Concept;

            var fromAccount = await _accountRepository.GetByIdAsync(command.FromAccountId);
            var toAccount = await _accountRepository.GetByIdAsync(command.ToAccountId);

            if (fromAccount == null)
                throw new DomainException(ErrorCodes.ACCOUNT_NOT_FOUND, "Cuenta de origen no encontrada");

            if (toAccount == null)
                throw new DomainException(ErrorCodes.ACCOUNT_NOT_FOUND, "Cuenta de destino no encontrada");

            if (!fromAccount.IsActive)
                throw new DomainException(ErrorCodes.ACCOUNT_INACTIVE, "La cuenta de origen está inactiva");

            if (!toAccount.IsActive)
                throw new DomainException(ErrorCodes.TARGET_ACCOUNT_INACTIVE, "La cuenta de destino está inactiva");

            if (fromAccount.Balance < command.Amount)
                throw new DomainException(ErrorCodes.INSUFFICIENT_BALANCE, "Saldo insuficiente en la cuenta de origen");

            var (debitMovement, creditMovement) = fromAccount.Transfer(
                command.Amount,
                toAccount,
                concept
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
        catch (DomainException ex)
        {
            return new TransferResponse(
                OperationId: command.OperationId,
                Message: $"Error de negocio: {ex.Message}"
            );
        }
        catch (Exception ex)
        {
            return new TransferResponse(
                OperationId: command.OperationId,
                Message: $"Error interno del servidor: {ex.Message}"
            );
        }
    }

    public async Task<GetMovementsReportResponse> GetReportAsync(GetMovementsReportQuery query)
    {
        try
        {
            if (query.AccountId == Guid.Empty)
                throw new DomainException(ErrorCodes.ACCOUNT_ID_REQUIRED, "El ID de la cuenta es requerido");

            if (query.StartDate > query.EndDate)
                throw new DomainException(ErrorCodes.AMOUNT_INVALID, "La fecha de inicio no puede ser mayor a la fecha de fin");

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
        catch (DomainException ex)
        {
            return new GetMovementsReportResponse(
                OperationId: query.OperationId,
                Message: $"Error de negocio: {ex.Message}"
            );
        }
        catch (Exception ex)
        {
            return new GetMovementsReportResponse(
                OperationId: query.OperationId,
                Message: $"Error interno del servidor: {ex.Message}"
            );
        }
    }
}
