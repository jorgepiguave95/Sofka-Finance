namespace SofkaFinance.Contracts.Accounts;

public record CreateAccountResponse(
    Guid OperationId,
    string Message,
    Guid? AccountId = null
);

public record GetAccountResponse(
    Guid OperationId,
    string Message,
    object? Account = null
);

public record GetAllAccountsResponse(
    Guid OperationId,
    string Message,
    object[]? Accounts = null
);

public record GetAccountsByCustomerResponse(
    Guid OperationId,
    string Message,
    object[]? Accounts = null
);

public record CloseAccountResponse(
    Guid OperationId,
    string Message,
    Guid? AccountId = null
);
