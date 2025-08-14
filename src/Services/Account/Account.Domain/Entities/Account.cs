using Account.Domain.Enums;
using Account.Domain.Errors;
using Account.Domain.ValueObjects;

namespace Account.Domain.Entities;

public sealed class Account
{
    public Guid Id { get; private set; }
    public AccountNumber Number { get; private set; } = default!;
    public AccountType Type { get; private set; }
    public decimal InitialBalance { get; private set; }
    public decimal CurrentBalance { get; private set; }
    public bool IsActive { get; private set; }

    private Account() { } 

    private Account(AccountNumber number, AccountType type, decimal initialBalance, bool isActive)
    {
        if (initialBalance < 0)
            throw new DomainException("El saldo inicial no puede ser negativo.", ErrorCodes.INITIAL_BALANCE_INVALID);

        Id = Guid.NewGuid();
        Number = number ?? throw new DomainException("Se requiere el número de cuenta.", ErrorCodes.ACCOUNT_NUMBER_REQUIRED);
        Type = type;
        InitialBalance = initialBalance;
        CurrentBalance = initialBalance;
        IsActive = isActive;
    }

    public static Account Open(AccountNumber number, AccountType type, decimal initialBalance = 0m, bool isActive = true)
        => new(number, type, initialBalance, isActive);

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    public Movement RegisterMovement(MovementType movementType, decimal amount, DateTime? whenUtc = null)
    {
        if (!IsActive)
            throw new DomainException("Cuenta Inactiva.", ErrorCodes.ACCOUNT_INACTIVE);

        if (amount == 0)
            throw new DomainException("El monto del movimiento no puede ser cero.", ErrorCodes.MOVEMENT_VALUE_ZERO);

        var signed = movementType == MovementType.Deposit ? Math.Abs(amount) : -Math.Abs(amount);
        var projected = CurrentBalance + signed;

        if (projected < 0)
            throw new DomainException("Saldo no disponible", ErrorCodes.INSUFFICIENT_BALANCE);

        CurrentBalance = projected;

        return Movement.Create(
            accountId: Id,
            dateUtc: whenUtc ?? DateTime.UtcNow,
            type: movementType,
            value: signed,
            postBalance: CurrentBalance
        );
    }
}
