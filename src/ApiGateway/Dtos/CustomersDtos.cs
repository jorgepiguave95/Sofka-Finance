namespace ApiGateway.Dtos;

public record CreateCustomerDto(
    string Name,
    string Gender,
    int Age,
    string Identification,
    string Address,
    string Phone,
    string Email,
    string Password
);

public record UpdateCustomerDto(
    string? Name,
    string? Address,
    string? Phone,
    string? Email
);
