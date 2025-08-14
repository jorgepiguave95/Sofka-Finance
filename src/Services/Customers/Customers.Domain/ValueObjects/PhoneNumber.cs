using System.Text.RegularExpressions;
using Customers.Domain.Errors;

namespace Customers.Domain.ValueObjects
{
    public class PhoneNumber
    {
        public string Value { get; }

        private static readonly Regex PhoneRegex = new(@"^\+?[0-9]{7,15}$", RegexOptions.Compiled);

        public PhoneNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Se requiere el número de teléfono.", ErrorCodes.CustomerPhoneRequired);

            if (!PhoneRegex.IsMatch(value))
                throw new DomainException("Formato de número de teléfono inválido.", ErrorCodes.CustomerPhoneInvalid);

            Value = value;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not PhoneNumber other) return false;
            return Value.Equals(other.Value);
        }

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value;
    }
}
