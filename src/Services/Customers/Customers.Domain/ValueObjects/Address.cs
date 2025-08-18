using Customers.Domain.Errors;

namespace Customers.Domain.ValueObjects;

public record Address
{
    private const int MinimumLength = 15;

    public string Value { get; init; } = default!;

    // Constructor privado para Entity Framework
    private Address() { }

    public Address(string value)
    {
        value = value?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("La dirección es requerida.", ErrorCodes.CustomerAddressRequired);

        if (value.Length < MinimumLength)
            throw new DomainException(
                $"La dirección debe tener al menos {MinimumLength} caracteres. Longitud actual: {value.Length}",
                ErrorCodes.CustomerAddressTooShort);

        Value = value;
    }

    // public static implicit operator string(Address address) => address.Value;

    public override string ToString() => Value;
}