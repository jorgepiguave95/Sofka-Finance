using Account.Application.Interfaces;
using SofkaFinance.Contracts.Accounts;

namespace Account.Api.Controllers;

public class MovementsController
{
    private readonly IMovementService _movementService;

    public MovementsController(IMovementService movementService)
    {
        _movementService = movementService;
    }

    public async Task<GetMovementsByAccountResponse> GetByAccount(GetMovementsByAccountQuery query)
    {
        return await _movementService.GetByAccountAsync(query);
    }

    public async Task<DepositResponse> Deposit(DepositCommand command)
    {
        return await _movementService.DepositAsync(command);
    }

    public async Task<WithdrawResponse> Withdraw(WithdrawCommand command)
    {
        return await _movementService.WithdrawAsync(command);
    }

    public async Task<TransferResponse> Transfer(TransferCommand command)
    {
        return await _movementService.TransferAsync(command);
    }

    public async Task<GetMovementsReportResponse> GetReport(GetMovementsReportQuery query)
    {
        return await _movementService.GetReportAsync(query);
    }
}
