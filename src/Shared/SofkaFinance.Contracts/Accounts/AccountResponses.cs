namespace SofkaFinance.Contracts.Accounts;

public record CreateAccountResponse(
    Guid OperationId,
    bool Success,
    string Message,
    Guid? AccountId = null
);

public record GetAccountResponse(
    Guid OperationId,
    bool Success,
    string Message,
    object? Account = null
);

public record GetAllAccountsResponse(
    Guid OperationId,
    bool Success,
    string Message,
    object[]? Accounts = null
);

public record GetAccountsByCustomerResponse(
    Guid OperationId,
    bool Success,
    string Message,
    object[]? Accounts = null
);

public record CloseAccountResponse(
    Guid OperationId,
    bool Success,
    string Message,
    Guid? AccountId = null
);
