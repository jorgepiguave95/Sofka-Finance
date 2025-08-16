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