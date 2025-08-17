using Account.Application.Interfaces;
using SofkaFinance.Contracts.Accounts;

namespace Account.Api.Controllers;

public class AccountsController
{
    private readonly IAccountService _accountService;

    public AccountsController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    public async Task<GetAccountResponse> GetById(GetAccountByIdQuery query)
    {
        return await _accountService.GetByIdAsync(query);
    }

    public async Task<GetAccountsByCustomerResponse> GetByCustomer(GetAccountsByCustomerQuery query)
    {
        return await _accountService.GetByCustomerAsync(query);
    }

    public async Task<GetAllAccountsResponse> GetAll(GetAllAccountsQuery query)
    {
        return await _accountService.GetAllAsync(query);
    }

    public async Task<CreateAccountResponse> Create(CreateAccountCommand command)
    {
        return await _accountService.CreateAsync(command);
    }

    public async Task<CloseAccountResponse> Close(CloseAccountCommand command)
    {
        return await _accountService.CloseAsync(command);
    }
}
