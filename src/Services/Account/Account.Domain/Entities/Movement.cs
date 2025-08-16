using Account.Domain.Errors;
using Account.Domain.ValueObjects;

namespace Account.Domain.Entities;

public sealed class Movement
{
    public Guid Id { get; private set; }
    public Guid AccountId { get; private set; }
    public MovementType Type { get; private set; } = default!;
    public decimal Value { get; private set; }
    public decimal AvailableBalance { get; private set; }
    public bool Status { get; private set; }
    public DateTime Date { get; private set; }
    public string? Concept { get; private set; }

    private Movement() { }

    private Movement(Guid accountId, DateTime dateUtc, MovementType type, decimal value, decimal availableBalance, string? concept)
    {
        if (accountId == Guid.Empty)
            throw new DomainException("El ID de la cuenta es requerido.", ErrorCodes.ACCOUNT_ID_REQUIRED);

        if (value == 0)
            throw new DomainException("El monto del movimiento no puede ser cero.", ErrorCodes.MOVEMENT_VALUE_ZERO);

        Id = Guid.NewGuid();
        AccountId = accountId;
        Date = dateUtc;
        Type = type ?? throw new DomainException("El tipo de movimiento es requerido.", ErrorCodes.MOVEMENT_TYPE_REQUIRED);

        Value = Math.Abs(value);

        AvailableBalance = availableBalance;
        Status = true;
        Concept = concept;
    }

    public static Movement Create(Guid accountId, DateTime dateUtc, MovementType type, decimal value, decimal availableBalance, string? concept = null)
        => new(accountId, dateUtc, type, value, availableBalance, concept);

}