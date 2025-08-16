namespace SofkaFinance.Contracts.Customers;

public record LoginCommand(
    Guid OperationId,
    string Email,
    string Password
);
