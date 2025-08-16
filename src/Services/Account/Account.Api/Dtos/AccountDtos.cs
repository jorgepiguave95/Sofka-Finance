namespace Account.Api.Dtos;

public record AccountDto(
    Guid AccountId,
    Guid CustomerId,
    string AccountNumber,
    string AccountType,
    decimal Balance,
    bool IsActive,
    DateTime CreatedAt
);

public record CreateAccountDto(
    Guid CustomerId,
    string AccountType,
    decimal InitialBalance = 0
);

public record DepositDto(
    decimal Amount,
    string? Concept
);

public record WithdrawDto(
    decimal Amount,
    string? Concept
);

public record TransferDto(
    Guid FromAccountId,
    Guid ToAccountId,
    decimal Amount,
    string? Concept
);
