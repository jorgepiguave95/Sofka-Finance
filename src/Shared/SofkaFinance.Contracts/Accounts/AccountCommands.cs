namespace SofkaFinance.Contracts.Accounts;

public record CreateAccountCommand(
    Guid OperationId,
    Guid CustomerId,
    string AccountType,
    string? AccountNumber = null,
    decimal InitialBalance = 0
);

public record CloseAccountCommand(
    Guid OperationId,
    Guid AccountId
);

