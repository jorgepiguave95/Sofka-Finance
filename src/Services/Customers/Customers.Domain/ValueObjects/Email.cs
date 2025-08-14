using System.Text.RegularExpressions;
using Customers.Domain.Errors;

namespace Customers.Domain.ValueObjects
{
    public class Email
    {
        public string Value { get; }

        private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Se requiere el correo electrónico.", ErrorCodes.CustomerEmailRequired);

            if (!EmailRegex.IsMatch(value))
                throw new DomainException("Formato de correo electrónico inválido.", ErrorCodes.CustomerEmailInvalid);

            Value = value;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Email other) return false;
            return Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode() => Value.ToLower().GetHashCode();

        public override string ToString() => Value;
    }
}
