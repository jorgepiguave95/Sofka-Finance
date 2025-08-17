using SofkaFinance.Contracts.Accounts;

namespace Account.Application.Interfaces;

public interface IMovementService
{
    Task<GetMovementsByAccountResponse> GetByAccountAsync(GetMovementsByAccountQuery query);
    Task<DepositResponse> DepositAsync(DepositCommand command);
    Task<WithdrawResponse> WithdrawAsync(WithdrawCommand command);
    Task<TransferResponse> TransferAsync(TransferCommand command);
    Task<GetMovementsReportResponse> GetReportAsync(GetMovementsReportQuery query);
}