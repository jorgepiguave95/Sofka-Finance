namespace SofkaFinance.Contracts.Accounts;

// Movement Command Responses
public record DepositResponse(
    Guid OperationId,
    bool Success,
    string Message,
    Guid? MovementId = null,
    decimal? NewBalance = null
);

public record WithdrawResponse(
    Guid OperationId,
    bool Success,
    string Message,
    Guid? MovementId = null,
    decimal? NewBalance = null
);

public record TransferResponse(
    Guid OperationId,
    bool Success,
    string Message,
    Guid? MovementId = null,
    decimal? FromAccountNewBalance = null,
    decimal? ToAccountNewBalance = null
);

// Movement Query Responses
public record GetMovementsByAccountResponse(
    Guid OperationId,
    bool Success,
    string Message,
    object[]? Movements = null
);

public record GetMovementsReportResponse(
    Guid OperationId,
    bool Success,
    string Message,
    object? Report = null,
    object[]? Movements = null
);
