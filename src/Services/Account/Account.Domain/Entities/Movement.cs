using Account.Domain.Enums;
using Account.Domain.Errors;

namespace Account.Domain.Entities;

public sealed class Movement
{
    public Guid Id { get; private set; }
    public Guid AccountId { get; private set; }
    public MovementType Type { get; private set; }
    public decimal Value { get; private set; }
    public decimal AvailableBalance { get; private set; }
    public bool Status { get; private set; }
    public DateTime Date { get; private set; }
    public string? Concept { get; private set; }

    private Movement() { }

    private Movement(Guid accountId, DateTime dateUtc, MovementType type, decimal value, decimal availableBalance, string? concept)
    {
        if (value == 0)
            throw new DomainException("El monto del movimiento no puede ser cero.", ErrorCodes.MOVEMENT_VALUE_ZERO);

        Id = Guid.NewGuid();
        AccountId = accountId;
        Date = dateUtc;
        Type = type;
        Value = value;
        AvailableBalance = availableBalance;
        Status = true;
        Concept = concept;
    }

    public static Movement Create(Guid accountId, DateTime dateUtc, MovementType type, decimal value, decimal availableBalance, string? concept = null)
        => new(accountId, dateUtc, type, value, availableBalance, concept);
}
