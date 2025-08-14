namespace Customers.Domain.Errors;

public static class ErrorCodes
{
    // ======= Clientes =======
    public const string CustomerNotFound = "CUSTOMER_NOT_FOUND";
    public const string CustomerAlreadyExists = "CUSTOMER_ALREADY_EXISTS";
    public const string CustomerInactive = "CUSTOMER_INACTIVE";

    // ======= Nombre =======
    public const string CustomerNameRequired = "CUSTOMER_NAME_REQUIRED";
    public const string CustomerNameTooShort = "CUSTOMER_NAME_TOO_SHORT";
    public const string CustomerNameTooLong = "CUSTOMER_NAME_TOO_LONG";

    // ======= Email =======
    public const string CustomerEmailRequired = "CUSTOMER_EMAIL_REQUIRED";
    public const string CustomerEmailInvalid = "CUSTOMER_EMAIL_INVALID";
    public const string CustomerEmailAlreadyUsed = "CUSTOMER_EMAIL_ALREADY_USED";

    // ======= Teléfono =======
    public const string CustomerPhoneRequired = "CUSTOMER_PHONE_REQUIRED";
    public const string CustomerPhoneInvalid = "CUSTOMER_PHONE_INVALID";
    public const string CustomerPhoneAlreadyUsed = "CUSTOMER_PHONE_ALREADY_USED";

    // ======= Dirección =======
    public const string CustomerAddressRequired = "CUSTOMER_ADDRESS_REQUIRED";
    public const string CustomerAddressTooLong = "CUSTOMER_ADDRESS_TOO_LONG";

    // ======= Contraseña =======
    public const string CustomerPasswordRequired = "CUSTOMER_PASSWORD_REQUIRED";
    public const string CustomerPasswordTooShort = "CUSTOMER_PASSWORD_TOO_SHORT";
    public const string CustomerPasswordTooWeak = "CUSTOMER_PASSWORD_TOO_WEAK";

    // ======= Fechas =======
    public const string CustomerBirthDateRequired = "CUSTOMER_BIRTHDATE_REQUIRED";
    public const string CustomerBirthDateInvalid = "CUSTOMER_BIRTHDATE_INVALID";
    public const string CustomerUnderAge = "CUSTOMER_UNDER_AGE";

    // ======= Credenciales / Seguridad =======
    public const string InvalidCredentials = "INVALID_CREDENTIALS";
    public const string UnauthorizedAccess = "UNAUTHORIZED_ACCESS";

    // ======= Infraestructura =======
    public const string DatabaseError = "DATABASE_ERROR";
    public const string ExternalServiceUnavailable = "EXTERNAL_SERVICE_UNAVAILABLE";

    // ======= Persona =======
    public const string PersonNameRequired = "PERSON_NAME_REQUIRED";
    public const string PersonAgeInvalid = "PERSON_AGE_INVALID";
    public const string PersonIdentificationRequired = "PERSON_IDENTIFICATION_REQUIRED";
    public const string PersonAddressRequired = "PERSON_ADDRESS_REQUIRED";
    public const string PersonPhoneRequired = "PERSON_PHONE_REQUIRED";
}