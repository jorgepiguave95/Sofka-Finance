namespace SofkaFinance.Contracts.Accounts;

// Movement Command Responses
public record DepositResponse(
    Guid OperationId,
    string Message,
    Guid? MovementId = null,
    decimal? NewBalance = null
);

public record WithdrawResponse(
    Guid OperationId,
    string Message,
    Guid? MovementId = null,
    decimal? NewBalance = null
);

public record TransferResponse(
    Guid OperationId,
    string Message,
    Guid? MovementId = null,
    decimal? FromAccountNewBalance = null,
    decimal? ToAccountNewBalance = null
);

// Movement Query Responses
public record GetMovementsByAccountResponse(
    Guid OperationId,
    string Message,
    object[]? Movements = null
);

public record GetMovementsReportResponse(
    Guid OperationId,
    string Message,
    object? Report = null,
    object[]? Movements = null
);
