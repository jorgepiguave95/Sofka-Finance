namespace SofkaFinance.Contracts.Accounts;

public record DepositCommand(
    Guid OperationId,
    Guid AccountId,
    decimal Amount,
    string? Concept
);

public record WithdrawCommand(
    Guid OperationId,
    Guid AccountId,
    decimal Amount,
    string? Concept
);

public record TransferCommand(
    Guid OperationId,
    Guid FromAccountId,
    Guid ToAccountId,
    decimal Amount,
    string? Concept
);