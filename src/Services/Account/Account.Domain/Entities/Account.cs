using Account.Domain.Enums;
using Account.Domain.Errors;
using Account.Domain.ValueObjects;

namespace Account.Domain.Entities;

public sealed class Account
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public AccountNumber Number { get; private set; } = default!;
    public AccountType Type { get; private set; }
    public decimal Balance { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Account() { }

    private Account(Guid customerId, AccountNumber number, AccountType type, decimal initialBalance, bool isActive)
    {
        if (initialBalance < 0)
            throw new DomainException("El saldo inicial no puede ser negativo.", ErrorCodes.INITIAL_BALANCE_INVALID);

        Id = Guid.NewGuid();
        CustomerId = customerId;
        Number = number ?? throw new DomainException("Se requiere el número de cuenta.", ErrorCodes.ACCOUNT_NUMBER_REQUIRED);
        Type = type;
        Balance = initialBalance;
        IsActive = isActive;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Account Open(Guid customerId, AccountNumber number, AccountType type, decimal initialBalance = 0m, bool isActive = true)
        => new(customerId, number, type, initialBalance, isActive);

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public Movement RegisterMovement(MovementType movementType, decimal amount, DateTime? whenUtc = null)
    {
        if (!IsActive)
            throw new DomainException("Cuenta Inactiva.", ErrorCodes.ACCOUNT_INACTIVE);

        if (amount == 0)
            throw new DomainException("El monto del movimiento no puede ser cero.", ErrorCodes.MOVEMENT_VALUE_ZERO);

        var signed = movementType == MovementType.Deposit ? Math.Abs(amount) : -Math.Abs(amount);
        var projected = Balance + signed;

        if (projected < 0)
            throw new DomainException("Saldo no disponible", ErrorCodes.INSUFFICIENT_BALANCE);

        Balance = projected;
        UpdatedAt = DateTime.UtcNow;

        return Movement.Create(
            accountId: Id,
            dateUtc: whenUtc ?? DateTime.UtcNow,
            type: movementType,
            value: signed,
            availableBalance: Balance
        );
    }
}
