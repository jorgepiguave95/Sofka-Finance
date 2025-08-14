using Account.Domain.Errors;

namespace Account.Domain.ValueObjects;

public sealed class AccountNumber
{
    public string Value { get; }

    private AccountNumber(string value)
    {
        value = value?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Se requiere el número de cuenta.", ErrorCodes.ACCOUNT_NUMBER_REQUIRED);

        Value = value;
    }

    public static AccountNumber Create(string value) => new(value);

    public override string ToString() => Value;
    public override bool Equals(object? obj) => obj is AccountNumber other && other.Value == Value;
    public override int GetHashCode() => Value.GetHashCode();
}
