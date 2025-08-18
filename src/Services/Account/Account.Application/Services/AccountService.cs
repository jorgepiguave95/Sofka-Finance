using Account.Application.Interfaces;
using Account.Application.Repositories;
using Account.Domain.Entities;
using Account.Domain.ValueObjects;
using SofkaFinance.Contracts.Accounts;
using Account.Domain.Errors;

namespace Account.Application.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;

    public AccountService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<GetAccountResponse> GetByIdAsync(GetAccountByIdQuery query)
    {
        try
        {
            if (query.AccountId == Guid.Empty)
                throw new DomainException(ErrorCodes.ACCOUNT_ID_REQUIRED, "El ID de la cuenta es requerido");

            var account = await _accountRepository.GetByIdAsync(query.AccountId);

            if (account == null)
                throw new DomainException(ErrorCodes.ACCOUNT_NOT_FOUND, "Cuenta no encontrada");

            var accountObject = new
            {
                Id = account.Id,
                IdCliente = account.CustomerId,
                NumeroCuenta = account.Number.Value,
                TipoCuenta = account.Type.Value,
                Saldo = account.Balance,
                EstaActiva = account.IsActive,
                FechaCreacion = account.CreatedAt
            };

            return new GetAccountResponse(
                OperationId: query.OperationId,
                Message: "Cuenta encontrada exitosamente",
                Account: accountObject
            );
        }
        catch (DomainException ex)
        {
            return new GetAccountResponse(
                OperationId: query.OperationId,
                Message: $"Error de negocio: {ex.Message}",
                Account: null
            );
        }
        catch (Exception ex)
        {
            return new GetAccountResponse(
                OperationId: query.OperationId,
                Message: $"Error interno del servidor: {ex.Message}",
                Account: null
            );
        }
    }

    public async Task<GetAccountsByCustomerResponse> GetByCustomerAsync(GetAccountsByCustomerQuery query)
    {
        try
        {
            if (query.CustomerId == Guid.Empty)
                throw new DomainException(ErrorCodes.CUSTOMER_ID_REQUIRED, "El ID del cliente es requerido");

            var accounts = await _accountRepository.GetByCustomerIdAsync(query.CustomerId);

            var accountObjects = accounts.Select(account => new
            {
                AccountId = account.Id,
                CustomerId = account.CustomerId,
                AccountNumber = account.Number.Value,
                AccountType = account.Type.Value,
                Balance = account.Balance,
                IsActive = account.IsActive,
                CreatedAt = account.CreatedAt
            }).ToArray();

            return new GetAccountsByCustomerResponse(
                OperationId: query.OperationId,
                Message: "Cuentas del cliente obtenidas exitosamente",
                Accounts: accountObjects
            );
        }
        catch (DomainException ex)
        {
            return new GetAccountsByCustomerResponse(
                OperationId: query.OperationId,
                Message: $"Error de negocio: {ex.Message}",
                Accounts: Array.Empty<object>()
            );
        }
        catch (Exception ex)
        {
            return new GetAccountsByCustomerResponse(
                OperationId: query.OperationId,
                Message: $"Error interno del servidor: {ex.Message}",
                Accounts: Array.Empty<object>()
            );
        }
    }

    public async Task<GetAllAccountsResponse> GetAllAsync(GetAllAccountsQuery query)
    {
        try
        {
            var allAccounts = await _accountRepository.GetAllAsync();

            var accountObjects = allAccounts.Select(account => new
            {
                Id = account.Id,
                IdCliente = account.CustomerId,
                NumeroCuenta = account.Number.Value,
                TipoCuenta = account.Type.Value,
                Saldo = account.Balance,
                EstaActiva = account.IsActive,
                FechaCreacion = account.CreatedAt
            }).ToArray();

            return new GetAllAccountsResponse(
                OperationId: query.OperationId,
                Message: "Todas las cuentas obtenidas exitosamente",
                Accounts: accountObjects
            );
        }
        catch (DomainException ex)
        {
            return new GetAllAccountsResponse(
                OperationId: query.OperationId,
                Message: $"Error de negocio: {ex.Message}",
                Accounts: Array.Empty<object>()
            );
        }
        catch (Exception ex)
        {
            return new GetAllAccountsResponse(
                OperationId: query.OperationId,
                Message: $"Error interno del servidor: {ex.Message}",
                Accounts: Array.Empty<object>()
            );
        }
    }

    public async Task<CreateAccountResponse> CreateAsync(CreateAccountCommand command)
    {
        try
        {
            if (command.CustomerId == Guid.Empty)
                throw new DomainException(ErrorCodes.CUSTOMER_ID_REQUIRED, "El ID del cliente es requerido");

            if (string.IsNullOrWhiteSpace(command.AccountType))
                throw new DomainException(ErrorCodes.ACCOUNT_TYPE_REQUIRED, "El tipo de cuenta es requerido");

            if (command.AccountType != "Ahorro" && command.AccountType != "Corriente")
                throw new DomainException(ErrorCodes.ACCOUNT_TYPE_INVALID, "El tipo de cuenta debe ser 'Ahorro' o 'Corriente'");

            var accountNumber = !string.IsNullOrWhiteSpace(command.AccountNumber)
                ? command.AccountNumber
                : await _accountRepository.GenerateAccountNumberAsync();

            var existingAccountWithNumber = await _accountRepository.GetByNumberAsync(accountNumber);
            if (existingAccountWithNumber != null)
                throw new DomainException(ErrorCodes.ACCOUNT_NUMBER_ALREADY_EXISTS, $"Ya existe una cuenta con el número {accountNumber}");

            var account = Account.Domain.Entities.Account.Create(
                customerId: command.CustomerId,
                number: new AccountNumber(accountNumber),
                type: new AccountType(command.AccountType),
                initialBalance: command.InitialBalance
            );

            var createdAccount = await _accountRepository.CreateAsync(account);

            return new CreateAccountResponse(
                OperationId: command.OperationId,
                Message: "Cuenta creada exitosamente",
                AccountId: createdAccount.Id
            );
        }
        catch (DomainException ex)
        {
            return new CreateAccountResponse(
                OperationId: command.OperationId,
                Message: $"Error de negocio: {ex.Message}"
            );
        }
        catch (Exception ex)
        {
            return new CreateAccountResponse(
                OperationId: command.OperationId,
                Message: $"Error interno del servidor: {ex.Message}"
            );
        }
    }

    public async Task<CloseAccountResponse> CloseAsync(CloseAccountCommand command)
    {
        try
        {
            if (command.AccountId == Guid.Empty)
                throw new DomainException(ErrorCodes.ACCOUNT_ID_REQUIRED, "El ID de la cuenta es requerido");

            var account = await _accountRepository.GetByIdAsync(command.AccountId);

            if (account == null)
                throw new DomainException(ErrorCodes.ACCOUNT_NOT_FOUND, "Cuenta no encontrada");

            if (!account.IsActive)
                throw new DomainException(ErrorCodes.ACCOUNT_ALREADY_INACTIVE, "La cuenta ya está cerrada");

            if (account.Balance > 0)
                throw new DomainException(ErrorCodes.ACCOUNT_HAS_BALANCE, "No se puede cerrar una cuenta con saldo positivo");

            account.Deactivate();

            await _accountRepository.UpdateAsync(account);

            return new CloseAccountResponse(
                OperationId: command.OperationId,
                Message: "Cuenta cerrada exitosamente",
                AccountId: account.Id
            );
        }
        catch (DomainException ex)
        {
            return new CloseAccountResponse(
                OperationId: command.OperationId,
                Message: $"Error de negocio: {ex.Message}"
            );
        }
        catch (Exception ex)
        {
            return new CloseAccountResponse(
                OperationId: command.OperationId,
                Message: $"Error interno del servidor: {ex.Message}"
            );
        }
    }
}
