namespace SofkaFinance.Contracts.Customers;

public record CustomerData(
    Guid Id,
    string Name,
    string Gender,
    int Age,
    string Identification,
    string Address,
    string Phone,
    string Email,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CreateCustomerResponse(
    Guid OperationId,
    string Message,
    Guid? CustomerId = null
);

public record GetCustomerResponse(
    Guid OperationId,
    string Message,
    CustomerData? Customer = null
);

public record GetAllCustomersResponse(
    Guid OperationId,
    string Message,
    CustomerData[]? Customers = null
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
