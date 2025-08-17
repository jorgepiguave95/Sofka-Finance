using Account.Application.Interfaces;
using Account.Application.Repositories;
using Account.Domain.Entities;
using Account.Domain.ValueObjects;
using SofkaFinance.Contracts.Accounts;

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
            var account = await _accountRepository.GetByIdAsync(query.AccountId);

            if (account == null)
            {
                return new GetAccountResponse(
                    OperationId: query.OperationId,
                    Message: "Cuenta no encontrada",
                    Account: null
                );
            }

            var accountObject = new
            {
                AccountId = account.Id,
                CustomerId = account.CustomerId,
                AccountNumber = account.Number.Value,
                AccountType = account.Type.Value,
                Balance = account.Balance,
                IsActive = account.IsActive,
                CreatedAt = account.CreatedAt
            };

            return new GetAccountResponse(
                OperationId: query.OperationId,
                Message: "Cuenta encontrada exitosamente",
                Account: accountObject
            );
        }
        catch (Exception ex)
        {
            return new GetAccountResponse(
                OperationId: query.OperationId,
                Message: ex.Message,
                Account: null
            );
        }
    }

    public async Task<GetAccountsByCustomerResponse> GetByCustomerAsync(GetAccountsByCustomerQuery query)
    {
        try
        {
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
        catch (Exception ex)
        {
            return new GetAccountsByCustomerResponse(
                OperationId: query.OperationId,
                Message: ex.Message,
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
                AccountId = account.Id,
                CustomerId = account.CustomerId,
                AccountNumber = account.Number.Value,
                AccountType = account.Type.Value,
                Balance = account.Balance,
                IsActive = account.IsActive,
                CreatedAt = account.CreatedAt,
                UpdatedAt = account.UpdatedAt
            }).ToArray();

            return new GetAllAccountsResponse(
                OperationId: query.OperationId,
                Message: "Todas las cuentas obtenidas exitosamente",
                Accounts: accountObjects
            );
        }
        catch (Exception ex)
        {
            return new GetAllAccountsResponse(
                OperationId: query.OperationId,
                Message: ex.Message,
                Accounts: Array.Empty<object>()
            );
        }
    }

    public async Task<CreateAccountResponse> CreateAsync(CreateAccountCommand command)
    {
        try
        {
            if (command.CustomerId == Guid.Empty)
            {
                return new CreateAccountResponse(
                    OperationId: command.OperationId,
                    Message: "El ID del cliente es requerido"
                );
            }

            if (string.IsNullOrWhiteSpace(command.AccountType))
            {
                return new CreateAccountResponse(
                    OperationId: command.OperationId,
                    Message: "El tipo de cuenta es requerido"
                );
            }

            var existingAccounts = await _accountRepository.GetByCustomerIdAsync(command.CustomerId);
            var hasAccountOfSameType = existingAccounts.Any(acc =>
                acc.Type.Value.Equals(command.AccountType, StringComparison.OrdinalIgnoreCase) &&
                acc.IsActive);

            if (hasAccountOfSameType)
            {
                return new CreateAccountResponse(
                    OperationId: command.OperationId,
                    Message: $"El cliente ya tiene una cuenta activa de tipo {command.AccountType}"
                );
            }

            var accountNumber = await _accountRepository.GenerateAccountNumberAsync();

            var account = Account.Domain.Entities.Account.Create(
                customerId: command.CustomerId,
                number: new AccountNumber(accountNumber),
                type: new AccountType(command.AccountType),
                initialBalance: 0m
            );

            var createdAccount = await _accountRepository.CreateAsync(account);

            return new CreateAccountResponse(
                OperationId: command.OperationId,
                Message: "Cuenta creada exitosamente",
                AccountId: createdAccount.Id
            );
        }
        catch (Exception ex)
        {
            return new CreateAccountResponse(
                OperationId: command.OperationId,
                Message: ex.Message
            );
        }
    }

    public async Task<CloseAccountResponse> CloseAsync(CloseAccountCommand command)
    {
        try
        {
            if (command.AccountId == Guid.Empty)
            {
                return new CloseAccountResponse(
                    OperationId: command.OperationId,
                    Message: "El ID de la cuenta es requerido"
                );
            }

            var account = await _accountRepository.GetByIdAsync(command.AccountId);

            if (account == null)
            {
                return new CloseAccountResponse(
                    OperationId: command.OperationId,
                    Message: "Cuenta no encontrada"
                );
            }

            if (!account.IsActive)
            {
                return new CloseAccountResponse(
                    OperationId: command.OperationId,
                    Message: "La cuenta ya está cerrada"
                );
            }

            account.Deactivate();

            await _accountRepository.UpdateAsync(account);

            return new CloseAccountResponse(
                OperationId: command.OperationId,
                Message: "Cuenta cerrada exitosamente",
                AccountId: account.Id
            );
        }
        catch (Exception ex)
        {
            return new CloseAccountResponse(
                OperationId: command.OperationId,
                Message: ex.Message
            );
        }
    }
}
