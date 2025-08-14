using Customers.Domain.Errors;
using Customers.Domain.ValueObjects;

namespace Customers.Domain.Entities;

public sealed class Client : Person
{
    public Email Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public bool IsActive { get; private set; }

    private Client() { }

    private Client(Guid id, string name, string gender, int age,
                   string identification, string address, PhoneNumber phone,
                   Email email, string passwordHash, bool isActive)
        : base(id, name, gender, age, identification, address, phone)
    {
        Email = email ?? throw new DomainException("Se requiere correo electrónico.", ErrorCodes.CustomerEmailRequired);

        passwordHash = passwordHash?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("Se requiere contraseña.", ErrorCodes.CustomerPasswordRequired);

        PasswordHash = passwordHash;
        IsActive = isActive;
    }

    public static Client Create(string name, string gender, int age,
                                string identification, string address, PhoneNumber phone,
                                Email email, string passwordHash)
        => new(Guid.NewGuid(), name, gender, age, identification, address, phone, email, passwordHash, true);

    public void ChangeEmail(Email newEmail)
        => Email = newEmail ?? throw new DomainException("Se requiere correo electrónico.", ErrorCodes.CustomerEmailRequired);

    public void ChangePassword(string newPasswordHash)
    {
        newPasswordHash = newPasswordHash?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new DomainException("Se requiere contraseña.", ErrorCodes.CustomerPasswordRequired);

        PasswordHash = newPasswordHash;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
