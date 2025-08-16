namespace Account.Api.Controllers;

using Account.Api.Dtos;
using Account.Api.Messaging;
using SofkaFinance.Contracts.Accounts;
public class AccountsController
{
    private readonly IMessagingClient _messagingClient;

    public AccountsController(IMessagingClient messagingClient)
    {
        _messagingClient = messagingClient;
    }

    public async Task<GetAccountResponse> GetById(GetAccountByIdQuery query)
    {
        try
        {
            // TODO: Implementar lógica de negocio real con base de datos

            Console.WriteLine($"GetById query processed for AccountId: {query.AccountId}");

            // Simular datos por ahora (reemplazar con lógica real de BD)
            var account = new AccountDto(
                AccountId: query.AccountId,
                CustomerId: Guid.NewGuid(), // Esto vendría de la BD
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
            Console.WriteLine($"Error getting account: {ex.Message}");
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
            Console.WriteLine($"GetByCustomer query processed for CustomerId: {query.CustomerId}");

            // Simular datos (reemplazar con lógica real de BD)
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
            Console.WriteLine($"Error getting accounts by customer: {ex.Message}");
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
            // TODO: Implementar lógica de creación de cuenta

            // Generar un AccountId único para la nueva cuenta
            var accountId = Guid.NewGuid();
            var accountNumber = $"ACC-{DateTime.UtcNow:yyyyMMdd}-{accountId.ToString()[..8].ToUpper()}";

            Console.WriteLine($"Account {accountId} created for customer {command.CustomerId}");

            // Retornar respuesta exitosa
            return new CreateAccountResponse(
                OperationId: command.OperationId,
                Success: true,
                Message: "Account created successfully",
                AccountId: accountId
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating account: {ex.Message}");
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
            Console.WriteLine($"Close account command processed for AccountId: {command.AccountId}");

            // TODO: Implementar lógica real de cierre de cuenta

            return new CloseAccountResponse(
                OperationId: command.OperationId,
                Success: true,
                Message: "Account closed successfully",
                AccountId: command.AccountId
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error closing account: {ex.Message}");
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
            Console.WriteLine($"Processing GetAllAccounts - OperationId: {query.OperationId}");

            // Simular espera asíncrona
            await Task.Delay(1);

            // Simulamos datos de cuentas
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
            Console.WriteLine($"Error getting all accounts: {ex.Message}");
            return new GetAllAccountsResponse(
                OperationId: query.OperationId,
                Success: false,
                Message: ex.Message,
                Accounts: null
            );
        }
    }
}
