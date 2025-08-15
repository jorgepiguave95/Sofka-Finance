namespace Customers;

// Comandos
public record CreateCustomerCommand(
    string FirstName,
    string LastName,
    string DocumentType,
    string DocumentNumber,
    string Email,
    string Phone
);

public record UpdateCustomerCommand(
    Guid CustomerId,
    string? FirstName,
    string? LastName,
    string? Email,
    string? Phone
);

public record DeleteCustomerCommand(Guid CustomerId);

// Consultas
public record GetCustomerByIdQuery(Guid CustomerId);

public record GetCustomerByIdResult(
    Guid CustomerId,
    string FirstName,
    string LastName,
    string DocumentType,
    string DocumentNumber,
    string Email,
    string? Phone
);

public record SearchCustomersQuery(string? Q, int Page = 1, int PageSize = 20);

public record SearchCustomersResult(IReadOnlyList<CustomerListItem> Customers);

public record CustomerListItem(
    Guid Id,
    string FirstName,
    string LastName,
    string DocumentNumber,
    string Email
);
