using Customers.Domain.Errors;
using Customers.Domain.ValueObjects;

namespace Customers.Domain.Entities;

public sealed class Client : Person
{
    public Email Email { get; private set; } = default!;
    public string Password { get; private set; } = default!;
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

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

        Password = passwordHash;
        IsActive = isActive;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Client Create(string name, string gender, int age,
                                string identification, string address, PhoneNumber phone,
                                Email email, string passwordHash)
        => new(Guid.NewGuid(), name, gender, age, identification, address, phone, email, passwordHash, true);

    public void ChangeEmail(Email newEmail)
    {
        Email = newEmail ?? throw new DomainException("Se requiere correo electrónico.", ErrorCodes.CustomerEmailRequired);
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangePassword(string newPasswordHash)
    {
        newPasswordHash = newPasswordHash?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new DomainException("Se requiere contraseña.", ErrorCodes.CustomerPasswordRequired);

        Password = newPasswordHash;
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
