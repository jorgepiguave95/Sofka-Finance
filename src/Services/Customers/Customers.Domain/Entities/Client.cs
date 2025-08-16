using Customers.Domain.Errors;
using Customers.Domain.ValueObjects;

namespace Customers.Domain.Entities;

public sealed class Client : Person
{
    public Email Email { get; private set; } = default!;
    public Password Password { get; private set; } = default!;
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Client() { }

    private Client(Guid id, string name, Gender gender, Age age,
                   string identification, Address address, PhoneNumber phone,
                   Email email, Password password, bool isActive)
        : base(id, name, gender, age, identification, address, phone)
    {
        Email = email ?? throw new DomainException("El correo electrónico es requerido.", ErrorCodes.CustomerEmailRequired);
        Password = password ?? throw new DomainException("La contraseña es requerida.", ErrorCodes.CustomerPasswordRequired);
        IsActive = isActive;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Client Create(string name, string gender, int age,
                                string identification, string address, string phone,
                                string email, string passwordHash)
    {
        var genderVO = new Gender(gender);
        var ageVO = new Age(age);
        var addressVO = new Address(address);
        var phoneVO = new PhoneNumber(phone);
        var emailVO = new Email(email);
        var passwordVO = new Password(passwordHash);

        return new Client(
            Guid.NewGuid(),
            name,
            genderVO,
            ageVO,
            identification,
            addressVO,
            phoneVO,
            emailVO,
            passwordVO,
            true
        );
    }

    public void ChangeEmail(string newEmail)
    {
        var emailVO = new Email(newEmail);
        Email = emailVO;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangePassword(string newPasswordHash)
    {
        var passwordVO = new Password(newPasswordHash);
        Password = passwordVO;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string name, string gender, int age, string address, string phone)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("El nombre es requerido.", ErrorCodes.CustomerNameRequired);

        var genderVO = new Gender(gender);
        var ageVO = new Age(age);
        var addressVO = new Address(address);
        var phoneVO = new PhoneNumber(phone);

        UpdateBasicInfo(name, genderVO, ageVO);
        UpdateAddress(addressVO);
        UpdatePhone(phoneVO);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}