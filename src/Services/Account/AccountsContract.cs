namespace Account;

// Comandos
public record CreateAccountCommand(
    Guid CustomerId,
    string AccountType,
    string Currency
);

public record DepositCommand(
    Guid AccountId,
    decimal Amount,
    string? Concept
);

public record WithdrawCommand(
    Guid AccountId,
    decimal Amount,
    string? Concept
);

public record TransferCommand(
    Guid FromAccountId,
    Guid ToAccountId,
    decimal Amount,
    string? Concept
);

public record CloseAccountCommand(Guid AccountId);

// Consultas
public record GetAccountByIdQuery(Guid AccountId);

public record GetAccountByIdResult(
    Guid AccountId,
    string AccountType,
    string Currency,
    decimal Balance
);

public record GetAccountsByCustomerQuery(Guid CustomerId, int Page = 1, int PageSize = 20);

public record GetAccountsByCustomerResult(IReadOnlyList<AccountListItem> Accounts);

public record AccountListItem(
    Guid Id,
    string AccountType,
    string Currency,
    decimal Balance
);

public record GetMovementsQuery(Guid AccountId, int Page = 1, int PageSize = 50);

public record GetMovementsResult(IReadOnlyList<AccountMovementItem> Movements);

public record AccountMovementItem(
    Guid Id,
    DateTimeOffset Date,
    string Type, // Ej: "DEPOSIT", "WITHDRAW"
    decimal Amount,
    string? Concept,
    decimal BalanceAfter
);
