namespace SofkaFinance.Contracts.Customers;

public record CreateCustomerCommand(
    Guid OperationId,
    string Name,
    string Gender,
    int Age,
    string Identification,
    string Address,
    string Phone,
    string Email,
    string Password
);

public record UpdateCustomerCommand(
    Guid OperationId,
    Guid CustomerId,
    string Name,
    string Email,
    string PhoneNumber,
    string Address
);

public record DeleteCustomerCommand(
    Guid OperationId,
    Guid CustomerId
);

