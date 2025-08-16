using System.Text.RegularExpressions;
using Customers.Domain.Errors;

namespace Customers.Domain.ValueObjects;

public record Email
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private const int MaxLength = 100;

    public string Value { get; }

    public Email(string value)
    {
        value = value?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("El correo electrónico es requerido.", ErrorCodes.CustomerEmailRequired);

        if (value.Length > MaxLength)
            throw new DomainException(
                $"El correo electrónico no puede exceder {MaxLength} caracteres. Longitud actual: {value.Length}",
                ErrorCodes.CustomerEmailTooLong);

        if (!EmailRegex.IsMatch(value))
            throw new DomainException(
                $"Formato de correo electrónico inválido: '{value}'",
                ErrorCodes.CustomerEmailInvalid);

        Value = value.ToLowerInvariant();
    }

    public static implicit operator string(Email email) => email.Value;

    public override string ToString() => Value;

    public virtual bool Equals(Email? other)
    {
        if (other is null) return false;
        return string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode() => Value.ToLowerInvariant().GetHashCode();
}