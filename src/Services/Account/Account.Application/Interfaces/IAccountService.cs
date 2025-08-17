using SofkaFinance.Contracts.Accounts;

namespace Account.Application.Interfaces;

public interface IAccountService
{
    Task<GetAccountResponse> GetByIdAsync(GetAccountByIdQuery query);
    Task<GetAccountsByCustomerResponse> GetByCustomerAsync(GetAccountsByCustomerQuery query);
    Task<GetAllAccountsResponse> GetAllAsync(GetAllAccountsQuery query);
    Task<CreateAccountResponse> CreateAsync(CreateAccountCommand command);
    Task<CloseAccountResponse> CloseAsync(CloseAccountCommand command);
}
