using Customers.Application.Interfaces;
using SofkaFinance.Contracts.Customers;

namespace Customers.Api.Controllers;

public class AuthController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<LoginResponse> Login(LoginCommand command)
    {
        return await _authService.LoginAsync(command);
    }
}
