using Customers;
using SofkaFinance.Contracts.Customers;

namespace Customers.Api.Controllers;

public class AuthController
{
    public Task Login(SofkaFinance.Contracts.Customers.LoginCommand command)
    {
        Console.WriteLine($"Login attempt for email: {command.Email}");
        return Task.CompletedTask;
    }
}
