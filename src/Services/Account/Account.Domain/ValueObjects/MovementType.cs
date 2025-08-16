using Account.Domain.Errors;

namespace Account.Domain.ValueObjects;

public record MovementType
{
    private static readonly string[] AllowedTypes = { "Depósito", "Retiro", "Transferencia" };

    public string Value { get; }

    public MovementType(string value)
    {
        value = value?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("El tipo de movimiento es requerido.", ErrorCodes.MOVEMENT_TYPE_REQUIRED);

        if (!AllowedTypes.Contains(value))
            throw new DomainException(
                $"Tipo de movimiento inválido. Debe ser uno de: {string.Join(", ", AllowedTypes)}. Valor recibido: '{value}'",
                ErrorCodes.MOVEMENT_TYPE_INVALID);

        Value = value;
    }

    public static implicit operator string(MovementType type) => type.Value;

    public override string ToString() => Value;
}