using Customers.Domain.Errors;

namespace Customers.Domain.ValueObjects;

public record Age
{
    private const int MinimumAge = 18;
    private const int MaximumAge = 90;

    public int Value { get; init; }

    // Constructor privado para Entity Framework
    private Age() { }

    public Age(int value)
    {
        if (value < MinimumAge)
            throw new DomainException(
                $"La edad debe ser mayor o igual a {MinimumAge} años. Edad actual: {value}",
                ErrorCodes.CustomerUnderAge);

        if (value > MaximumAge)
            throw new DomainException(
                $"La edad no puede ser mayor a {MaximumAge} años. Edad actual: {value}",
                ErrorCodes.CustomerAgeInvalid);

        Value = value;
    }

    // public static implicit operator int(Age age) => age.Value;

    public override string ToString() => Value.ToString();
}