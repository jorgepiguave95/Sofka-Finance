using Account.Domain.Errors;

namespace Account.Domain.ValueObjects;

public record AccountType
{
    private static readonly string[] AllowedTypes = { "Ahorro", "Corriente" };

    public string Value { get; }

    public AccountType(string value)
    {
        value = value?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("El tipo de cuenta es requerido.", ErrorCodes.ACCOUNT_TYPE_REQUIRED);

        if (!AllowedTypes.Contains(value))
            throw new DomainException(
                $"Tipo de cuenta inválido. Debe ser uno de: {string.Join(", ", AllowedTypes)}. Valor recibido: '{value}'",
                ErrorCodes.ACCOUNT_TYPE_INVALID);

        Value = value;
    }

    public static implicit operator string(AccountType type) => type.Value;

    public override string ToString() => Value;
}