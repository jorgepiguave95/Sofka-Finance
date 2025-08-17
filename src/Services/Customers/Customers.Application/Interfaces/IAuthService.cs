using SofkaFinance.Contracts.Customers;

namespace Customers.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginCommand command);
}
