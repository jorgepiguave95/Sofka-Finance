using Account.Domain.Enums;
using Account.Domain.Errors;

namespace Account.Domain.Entities;

public sealed class Movement
{
    public Guid Id { get; private set; }
    public Guid AccountId { get; private set; }
    public DateTime DateUtc { get; private set; }
    public MovementType Type { get; private set; }
    public decimal Value { get; private set; }
    public decimal PostBalance { get; private set; }

    private Movement() { } // EF

    private Movement(Guid accountId, DateTime dateUtc, MovementType type, decimal value, decimal postBalance)
    {
        if (value == 0)
            throw new DomainException("El monto del movimiento no puede ser cero.", ErrorCodes.MOVEMENT_VALUE_ZERO);

        Id = Guid.NewGuid();
        AccountId = accountId;
        DateUtc = dateUtc;
        Type = type;
        Value = value;
        PostBalance = postBalance;
    }

    public static Movement Create(Guid accountId, DateTime dateUtc, MovementType type, decimal value, decimal postBalance)
        => new(accountId, dateUtc, type, value, postBalance);
}
