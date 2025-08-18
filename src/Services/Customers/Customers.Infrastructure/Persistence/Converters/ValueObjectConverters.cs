using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Customers.Domain.ValueObjects;

namespace Customers.Infrastructure.Persistence.Converters;

public class EmailValueConverter : ValueConverter<Email, string>
{
    public EmailValueConverter() : base(
        email => email.Value,
        value => new Email(value))
    {
    }
}

public class GenderValueConverter : ValueConverter<Gender, string>
{
    public GenderValueConverter() : base(
        gender => gender.Value,
        value => new Gender(value))
    {
    }
}

public class AgeValueConverter : ValueConverter<Age, int>
{
    public AgeValueConverter() : base(
        age => age.Value,
        value => new Age(value))
    {
    }
}

public class AddressValueConverter : ValueConverter<Address, string>
{
    public AddressValueConverter() : base(
        address => address.Value,
        value => new Address(value))
    {
    }
}

public class PhoneNumberValueConverter : ValueConverter<PhoneNumber, string>
{
    public PhoneNumberValueConverter() : base(
        phone => phone.Value,
        value => new PhoneNumber(value))
    {
    }
}

public class PasswordValueConverter : ValueConverter<Password, string>
{
    public PasswordValueConverter() : base(
        password => password.Value,
        value => new Password(value))
    {
    }
}
