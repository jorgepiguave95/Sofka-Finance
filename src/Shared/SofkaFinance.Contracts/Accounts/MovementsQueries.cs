namespace SofkaFinance.Contracts.Accounts;

public record GetMovementsByAccountQuery(
    Guid OperationId,
    Guid AccountId,
    DateTime? StartDate = null,
    DateTime? EndDate = null
);

public record GetMovementsReportQuery(
    Guid OperationId,
    Guid AccountId,
    DateTime StartDate,
    DateTime EndDate
);
