namespace SofkaFinance.Contracts.Customers;

public record GetCustomerByIdQuery(
    Guid OperationId,
    Guid CustomerId
);

public record GetAllCustomersQuery(
    Guid OperationId
);

public record SearchCustomersQuery(
    Guid OperationId,
    string SearchTerm
);