using Customers.Domain.Errors;
using Customers.Domain.ValueObjects;

namespace Customers.Domain.Entities;

public abstract class Person
{
    public Guid Id { get; protected set; }
    public string Name { get; protected set; } = default!;
    public Gender Gender { get; protected set; } = default!;
    public Age Age { get; protected set; } = default!;
    public string Identification { get; protected set; } = default!;
    public Address Address { get; protected set; } = default!;
    public PhoneNumber Phone { get; protected set; } = default!;

    protected Person() { }

    protected Person(Guid id, string name, Gender gender, Age age,
                    string identification, Address address, PhoneNumber phone)
    {
        Id = id;

        name = name?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("El nombre es requerido.", ErrorCodes.CustomerNameRequired);

        if (name.Length < 3)
            throw new DomainException("El nombre debe tener al menos 3 caracteres.", ErrorCodes.CustomerNameTooShort);

        Name = name;
        Gender = gender ?? throw new DomainException("El género es requerido.", ErrorCodes.CustomerGenderRequired);
        Age = age ?? throw new DomainException("La edad es requerida.", ErrorCodes.CustomerAgeRequired);

        identification = identification?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(identification))
            throw new DomainException("La identificación es requerida.", ErrorCodes.CustomerIdentificationRequired);

        Identification = identification;
        Address = address ?? throw new DomainException("La dirección es requerida.", ErrorCodes.CustomerAddressRequired);
        Phone = phone ?? throw new DomainException("El teléfono es requerido.", ErrorCodes.CustomerPhoneRequired);
    }

    public void UpdateBasicInfo(string name, Gender gender, Age age)
    {
        name = name?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("El nombre es requerido.", ErrorCodes.CustomerNameRequired);

        Name = name;
        Gender = gender ?? throw new DomainException("El género es requerido.", ErrorCodes.CustomerGenderRequired);
        Age = age ?? throw new DomainException("La edad es requerida.", ErrorCodes.CustomerAgeRequired);
    }

    public void UpdateAddress(Address newAddress)
    {
        Address = newAddress ?? throw new DomainException("La dirección es requerida.", ErrorCodes.CustomerAddressRequired);
    }

    public void UpdatePhone(PhoneNumber newPhone)
    {
        Phone = newPhone ?? throw new DomainException("El teléfono es requerido.", ErrorCodes.CustomerPhoneRequired);
    }
}