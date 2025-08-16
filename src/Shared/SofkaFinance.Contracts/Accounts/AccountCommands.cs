namespace SofkaFinance.Contracts.Accounts;

public record CreateAccountCommand(
    Guid OperationId,
    Guid CustomerId,
    string AccountType
);

public record CloseAccountCommand(
    Guid OperationId,
    Guid AccountId
);

