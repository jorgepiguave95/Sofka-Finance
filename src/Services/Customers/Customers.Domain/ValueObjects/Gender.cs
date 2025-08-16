using Customers.Domain.Errors;

namespace Customers.Domain.ValueObjects;

public record Gender
{
    private static readonly string[] ValidGenders = { "Masculino", "Femenino" };

    public string Value { get; }

    public Gender(string value)
    {
        value = value?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("El género es requerido.", ErrorCodes.CustomerGenderRequired);

        value = char.ToUpper(value[0]) + value.Substring(1).ToLower();

        if (!ValidGenders.Contains(value))
            throw new DomainException(
                $"El género debe ser 'Masculino' o 'Femenino'. Valor recibido: '{value}'",
                ErrorCodes.CustomerGenderInvalid);

        Value = value;
    }

    public static implicit operator string(Gender gender) => gender.Value;

    public override string ToString() => Value;
}