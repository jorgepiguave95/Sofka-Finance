namespace SofkaFinance.Contracts.Customers;

public record CreateCustomerResponse(
    Guid OperationId,
    bool Success,
    string Message,
    Guid? CustomerId = null
);

public record GetCustomerResponse(
    Guid OperationId,
    bool Success,
    string Message,
    object? Customer = null
);

public record GetAllCustomersResponse(
    Guid OperationId,
    bool Success,
    string Message,
    object[]? Customers = null
);

public record UpdateCustomerResponse(
    Guid OperationId,
    bool Success,
    string Message,
    Guid? CustomerId = null
);

public record DeleteCustomerResponse(
    Guid OperationId,
    bool Success,
    string Message,
    Guid? CustomerId = null
);
