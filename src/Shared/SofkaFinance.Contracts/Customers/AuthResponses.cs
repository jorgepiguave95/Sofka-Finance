namespace SofkaFinance.Contracts.Customers;

public record LoginResponse(
    Guid OperationId,
    bool Success,
    string Message,
    string? Token = null,
    Guid? CustomerId = null,
    string? CustomerEmail = null,
    DateTime? ExpiresAt = null
);

public record ValidateTokenResponse(
    Guid OperationId,
    bool Success,
    string Message,
    bool IsValid = false,
    Guid? CustomerId = null,
    string? CustomerEmail = null,
    DateTime? ExpiresAt = null
);
