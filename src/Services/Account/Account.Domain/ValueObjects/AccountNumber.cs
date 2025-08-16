using System.Text.RegularExpressions;
using Account.Domain.Errors;

namespace Account.Domain.ValueObjects;

public record AccountNumber
{
    private static readonly Regex NumberRegex = new(@"^[0-9]+$", RegexOptions.Compiled);
    private const int MinLength = 8;
    private const int MaxLength = 10;

    public string Value { get; }

    public AccountNumber(string value)
    {
        value = value?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("El número de cuenta es requerido.", ErrorCodes.ACCOUNT_NUMBER_REQUIRED);

        if (!NumberRegex.IsMatch(value))
            throw new DomainException(
                $"El número de cuenta solo debe contener dígitos (0-9). Valor recibido: '{value}'",
                ErrorCodes.ACCOUNT_NUMBER_INVALID);

        if (value.Length < MinLength)
            throw new DomainException(
                $"El número de cuenta debe tener al menos {MinLength} dígitos. Longitud actual: {value.Length}",
                ErrorCodes.ACCOUNT_NUMBER_TOO_SHORT);

        if (value.Length > MaxLength)
            throw new DomainException(
                $"El número de cuenta no puede exceder {MaxLength} dígitos. Longitud actual: {value.Length}",
                ErrorCodes.ACCOUNT_NUMBER_TOO_LONG);

        Value = value;
    }

    public static implicit operator string(AccountNumber number) => number.Value;

    public override string ToString() => Value;

}