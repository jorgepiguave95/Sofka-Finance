namespace SofkaFinance.Contracts.Accounts;

public record GetAccountByIdQuery(
    Guid OperationId,
    Guid AccountId
);

public record GetAccountsByCustomerQuery(
    Guid OperationId,
    Guid CustomerId
);

public record GetAllAccountsQuery(
    Guid OperationId
);