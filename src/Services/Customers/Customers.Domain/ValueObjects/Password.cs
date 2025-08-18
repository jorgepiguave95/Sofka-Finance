using Customers.Domain.Errors;

namespace Customers.Domain.ValueObjects;

public record Password
{
    private const int MinimumLength = 8;

    public string Value { get; init; } = default!;

    // Constructor privado para Entity Framework
    private Password() { }

    public Password(string value)
    {
        value = value?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("La contraseña es requerida.", ErrorCodes.CustomerPasswordRequired);

        if (value.Length < MinimumLength)
            throw new DomainException(
                $"La contraseña debe tener mínimo {MinimumLength} caracteres. Longitud actual: {value.Length}",
                ErrorCodes.CustomerPasswordTooShort);

        Value = value;
    }

    // public static implicit operator string(Password password) => password.Value;

    public override string ToString() => "********";
}