namespace Account.Api.Controllers;

using Account.Api.Dtos;
using SofkaFinance.Contracts.Accounts;
public class AccountsController
{
    public AccountsController()
    {
    }

    public async Task<GetAccountResponse> GetById(GetAccountByIdQuery query)
    {
        try
        {
            await Task.Delay(1);

            var account = new AccountDto(
                AccountId: query.AccountId,
                CustomerId: Guid.NewGuid(),
                AccountNumber: $"ACC-20250815-{query.AccountId.ToString()[..8].ToUpper()}",
                AccountType: "Savings",
                Balance: 1500.00m,
                IsActive: true,
                CreatedAt: DateTime.UtcNow.AddDays(-30)
            );

            return new GetAccountResponse(
                OperationId: query.OperationId,
                Success: true,
                Message: "Account retrieved successfully",
                Account: account
            );
        }
        catch (Exception ex)
        {
            return new GetAccountResponse(
                OperationId: query.OperationId,
                Success: false,
                Message: ex.Message,
                Account: null
            );
        }
    }

    public async Task<GetAccountsByCustomerResponse> GetByCustomer(GetAccountsByCustomerQuery query)
    {
        try
        {
            await Task.Delay(1);

            var accounts = new List<AccountDto>
            {
                new AccountDto(
                    AccountId: Guid.NewGuid(),
                    CustomerId: query.CustomerId,
                    AccountNumber: $"ACC-20250815-AAAAAAAA",
                    AccountType: "Savings",
                    Balance: 2500.00m,
                    IsActive: true,
                    CreatedAt: DateTime.UtcNow.AddDays(-45)
                ),
                new AccountDto(
                    AccountId: Guid.NewGuid(),
                    CustomerId: query.CustomerId,
                    AccountNumber: $"ACC-20250815-BBBBBBBB",
                    AccountType: "Checking",
                    Balance: 750.50m,
                    IsActive: true,
                    CreatedAt: DateTime.UtcNow.AddDays(-20)
                )
            };

            return new GetAccountsByCustomerResponse(
                OperationId: query.OperationId,
                Success: true,
                Message: "Customer accounts retrieved successfully",
                Accounts: accounts.ToArray()
            );
        }
        catch (Exception ex)
        {
            return new GetAccountsByCustomerResponse(
                OperationId: query.OperationId,
                Success: false,
                Message: ex.Message,
                Accounts: null
            );
        }
    }

    public async Task<CreateAccountResponse> Create(CreateAccountCommand command)
    {
        try
        {
            var accountId = Guid.NewGuid();
            var accountNumber = $"ACC-{DateTime.UtcNow:yyyyMMdd}-{accountId.ToString()[..8].ToUpper()}";

            await Task.Delay(1);

            return new CreateAccountResponse(
                OperationId: command.OperationId,
                Success: true,
                Message: "Account created successfully",
                AccountId: accountId
            );
        }
        catch (Exception ex)
        {
            return new CreateAccountResponse(
                OperationId: command.OperationId,
                Success: false,
                Message: ex.Message,
                AccountId: null
            );
        }
    }

    public async Task<CloseAccountResponse> Close(CloseAccountCommand command)
    {
        try
        {
            await Task.Delay(1);

            return new CloseAccountResponse(
                OperationId: command.OperationId,
                Success: true,
                Message: "Account closed successfully",
                AccountId: command.AccountId
            );
        }
        catch (Exception ex)
        {
            return new CloseAccountResponse(
                OperationId: command.OperationId,
                Success: false,
                Message: ex.Message,
                AccountId: command.AccountId
            );
        }
    }

    public async Task<GetAllAccountsResponse> GetAll(GetAllAccountsQuery query)
    {
        try
        {
            await Task.Delay(1);

            var accounts = new List<AccountDto>
            {
                new AccountDto(
                    AccountId: Guid.NewGuid(),
                    CustomerId: Guid.NewGuid(),
                    AccountNumber: "ACC-20250815-GLOBAL01",
                    AccountType: "Savings",
                    Balance: 1500.00m,
                    IsActive: true,
                    CreatedAt: DateTime.UtcNow.AddDays(-30)
                ),
                new AccountDto(
                    AccountId: Guid.NewGuid(),
                    CustomerId: Guid.NewGuid(),
                    AccountNumber: "ACC-20250815-GLOBAL02",
                    AccountType: "Checking",
                    Balance: 2750.50m,
                    IsActive: true,
                    CreatedAt: DateTime.UtcNow.AddDays(-15)
                ),
                new AccountDto(
                    AccountId: Guid.NewGuid(),
                    CustomerId: Guid.NewGuid(),
                    AccountNumber: "ACC-20250815-GLOBAL03",
                    AccountType: "Savings",
                    Balance: 5000.00m,
                    IsActive: true,
                    CreatedAt: DateTime.UtcNow.AddDays(-60)
                )
            };

            return new GetAllAccountsResponse(
                OperationId: query.OperationId,
                Success: true,
                Message: "All accounts retrieved successfully",
                Accounts: accounts.ToArray()
            );
        }
        catch (Exception ex)
        {
            return new GetAllAccountsResponse(
                OperationId: query.OperationId,
                Success: false,
                Message: ex.Message,
                Accounts: null
            );
        }
    }
}
