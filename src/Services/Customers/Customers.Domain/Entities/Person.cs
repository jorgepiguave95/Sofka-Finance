using Customers.Domain.Errors;
using Customers.Domain.ValueObjects;

namespace Customers.Domain.Entities;

public abstract class Person
{
    public Guid Id { get; protected set; }
    public string Name { get; protected set; } = default!;
    public string Gender { get; protected set; } = default!;
    public int Age { get; protected set; }
    public string Identification { get; protected set; } = default!;
    public string Address { get; protected set; } = default!;
    public PhoneNumber Phone { get; protected set; } = default!;

    protected Person() { } 

    protected Person(Guid id, string name, string gender, int age,
                     string identification, string address, PhoneNumber phone)
    {
        name = name?.Trim() ?? string.Empty;
        identification = identification?.Trim() ?? string.Empty;
        address = address?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Se requiere el nombre.", ErrorCodes.PersonNameRequired);
        if (age < 0)
            throw new DomainException("La edad no puede ser negativa.", ErrorCodes.PersonAgeInvalid);
        if (string.IsNullOrWhiteSpace(identification))
            throw new DomainException("Se requiere la identificación.", ErrorCodes.PersonIdentificationRequired);
        if (string.IsNullOrWhiteSpace(address))
            throw new DomainException("Se requiere la dirección.", ErrorCodes.PersonAddressRequired);

        Id = id == default ? Guid.NewGuid() : id;
        Name = name;
        Gender = gender;
        Age = age;
        Identification = identification;
        Address = address;
        Phone = phone ?? throw new DomainException("Se requiere el teléfono.", ErrorCodes.PersonPhoneRequired);
    }

    public void Rename(string newName)
    {
        newName = newName?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(newName))
            throw new DomainException("Se requiere el nombre.", ErrorCodes.PersonNameRequired);

        Name = newName;
    }

    public void ChangePhone(PhoneNumber newPhone)
        => Phone = newPhone ?? throw new DomainException("Se requiere el teléfono.", ErrorCodes.PersonPhoneRequired);

    public void UpdateAddress(string newAddress)
    {
        newAddress = newAddress?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(newAddress))
            throw new DomainException("Se requiere la dirección.", ErrorCodes.PersonAddressRequired);

        Address = newAddress;
    }
}
