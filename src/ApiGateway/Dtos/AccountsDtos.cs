namespace ApiGateway.Dtos;

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

public record GetMovementReportDto(
    Guid IdCliente
);
