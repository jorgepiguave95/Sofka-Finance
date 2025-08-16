using System.Text.RegularExpressions;
using Customers.Domain.Errors;

namespace Customers.Domain.ValueObjects;

public record PhoneNumber
{
    private static readonly Regex PhoneRegex = new(
        @"^[0-9]+$",
        RegexOptions.Compiled);

    private const int MinDigits = 5;
    private const int MaxDigits = 10;

    public string Value { get; }

    public PhoneNumber(string value)
    {
        value = value?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("El número de teléfono es requerido.", ErrorCodes.CustomerPhoneRequired);

        if (!PhoneRegex.IsMatch(value))
            throw new DomainException(
                $"El número de teléfono solo debe contener dígitos (0-9). Valor recibido: '{value}'",
                ErrorCodes.CustomerPhoneInvalid);

        if (value.Length < MinDigits)
            throw new DomainException(
                $"El número de teléfono debe tener al menos {MinDigits} dígitos. Dígitos actuales: {value.Length}",
                ErrorCodes.CustomerPhoneTooShort);

        if (value.Length > MaxDigits)
            throw new DomainException(
                $"El número de teléfono no puede exceder {MaxDigits} dígitos. Dígitos actuales: {value.Length}",
                ErrorCodes.CustomerPhoneTooLong);

        Value = value;
    }

    public static implicit operator string(PhoneNumber phone) => phone.Value;

    public override string ToString() => Value;
}