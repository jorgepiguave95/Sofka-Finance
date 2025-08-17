namespace SofkaFinance.Contracts.Customers;

public record CreateCustomerResponse(
    Guid OperationId,
    string Message,
    Guid? CustomerId = null
);

public record GetCustomerResponse(
    Guid OperationId,
    string Message,
    object? Customer = null
);

public record GetAllCustomersResponse(
    Guid OperationId,
    string Message,
    object[]? Customers = null
);

public record UpdateCustomerResponse(
    Guid OperationId,
    string Message,
    Guid? CustomerId = null
);

public record DeleteCustomerResponse(
    Guid OperationId,
    string Message,
    Guid? CustomerId = null
);
