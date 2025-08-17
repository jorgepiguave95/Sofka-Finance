namespace SofkaFinance.Contracts.Customers;

public record LoginResponse(
    Guid OperationId,
    string Message,
    string? Token = null,
    Guid? CustomerId = null,
    string? CustomerEmail = null
);
