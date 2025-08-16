using Account.Domain.Errors;
using Account.Domain.ValueObjects;

namespace Account.Domain.Entities;

public sealed class Account
{
    private const decimal MinimumBalance = 0m;

    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public AccountNumber Number { get; private set; } = default!;
    public AccountType Type { get; private set; } = default!;
    public decimal Balance { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Account() { }

    private Account(Guid customerId, AccountNumber number, AccountType type, decimal initialBalance)
    {
        if (customerId == Guid.Empty)
            throw new DomainException("El ID del cliente es requerido.", ErrorCodes.CUSTOMER_ID_REQUIRED);

        if (initialBalance < MinimumBalance)
            throw new DomainException(
                $"El saldo inicial no puede ser menor a {MinimumBalance:C}. Valor recibido: {initialBalance:C}",
                ErrorCodes.INITIAL_BALANCE_INVALID);

        Id = Guid.NewGuid();
        CustomerId = customerId;
        Number = number ?? throw new DomainException("El número de cuenta es requerido.", ErrorCodes.ACCOUNT_NUMBER_REQUIRED);
        Type = type ?? throw new DomainException("El tipo de cuenta es requerido.", ErrorCodes.ACCOUNT_TYPE_REQUIRED);
        Balance = initialBalance;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = null;
    }

    public static Account Create(Guid customerId, AccountNumber number, AccountType type, decimal initialBalance = 0m)
    {
        return new Account(customerId, number, type, initialBalance);
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (Balance > 0)
            throw new DomainException(
                $"No se puede desactivar una cuenta con saldo positivo. Saldo actual: {Balance:C}",
                ErrorCodes.ACCOUNT_HAS_BALANCE);

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public Movement Deposit(decimal amount, string? concept = null)
    {
        ValidateActiveAccount();
        ValidateAmount(amount);

        Balance += amount;
        UpdatedAt = DateTime.UtcNow;

        return Movement.Create(
            accountId: Id,
            dateUtc: DateTime.UtcNow,
            type: new MovementType("Depósito"),
            value: amount,
            availableBalance: Balance,
            concept: concept ?? "Depósito"
        );
    }

    public Movement Withdraw(decimal amount, string? concept = null)
    {
        ValidateActiveAccount();
        ValidateAmount(amount);

        var projectedBalance = Balance - amount;
        if (projectedBalance < MinimumBalance)
            throw new DomainException(
                $"Saldo insuficiente. Saldo disponible: {Balance:C}, Monto solicitado: {amount:C}",
                ErrorCodes.INSUFFICIENT_BALANCE);

        Balance = projectedBalance;
        UpdatedAt = DateTime.UtcNow;

        return Movement.Create(
            accountId: Id,
            dateUtc: DateTime.UtcNow,
            type: new MovementType("Retiro"),
            value: amount,
            availableBalance: Balance,
            concept: concept ?? "Retiro"
        );
    }

    public (Movement debitMovement, Movement creditMovement) Transfer(
        decimal amount,
        Account targetAccount,
        string? concept = null)
    {
        if (targetAccount == null)
            throw new DomainException("La cuenta destino es requerida.", ErrorCodes.TARGET_ACCOUNT_REQUIRED);

        if (Id == targetAccount.Id)
            throw new DomainException("No se puede transferir a la misma cuenta.", ErrorCodes.SAME_ACCOUNT_TRANSFER);

        if (!targetAccount.IsActive)
            throw new DomainException("La cuenta destino está inactiva.", ErrorCodes.TARGET_ACCOUNT_INACTIVE);

        var debitMovement = Withdraw(amount, concept ?? $"Transferencia a cuenta {targetAccount.Number}");

        var creditMovement = targetAccount.Deposit(amount, concept ?? $"Transferencia desde cuenta {Number}");

        return (debitMovement, creditMovement);
    }

    private void ValidateActiveAccount()
    {
        if (!IsActive)
            throw new DomainException("La cuenta está inactiva.", ErrorCodes.ACCOUNT_INACTIVE);
    }

    private void ValidateAmount(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException(
                $"El monto debe ser mayor a cero",
                ErrorCodes.AMOUNT_INVALID);

        if (decimal.Round(amount, 2) != amount)
            throw new DomainException(
                $"El monto no puede tener más de 2 decimales. Valor recibido: {amount}",
                ErrorCodes.AMOUNT_DECIMAL_INVALID);
    }
}