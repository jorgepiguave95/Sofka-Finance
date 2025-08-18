namespace Customers.Domain.Errors;

public static class ErrorCodes
{
    // Customer Name
    public const string CustomerNameRequired = "CUSTOMER_NAME_REQUIRED";
    public const string CustomerNameTooShort = "CUSTOMER_NAME_TOO_SHORT";

    // Customer Gender
    public const string CustomerGenderRequired = "CUSTOMER_GENDER_REQUIRED";
    public const string CustomerGenderInvalid = "CUSTOMER_GENDER_INVALID";

    // Customer Age
    public const string CustomerAgeRequired = "CUSTOMER_AGE_REQUIRED";
    public const string CustomerUnderAge = "CUSTOMER_UNDER_AGE";
    public const string CustomerAgeInvalid = "CUSTOMER_AGE_INVALID";

    // Customer Identification
    public const string CustomerIdentificationRequired = "CUSTOMER_IDENTIFICATION_REQUIRED";

    // Customer Address
    public const string CustomerAddressRequired = "CUSTOMER_ADDRESS_REQUIRED";
    public const string CustomerAddressTooShort = "CUSTOMER_ADDRESS_TOO_SHORT";

    // Customer Phone
    public const string CustomerPhoneRequired = "CUSTOMER_PHONE_REQUIRED";
    public const string CustomerPhoneTooShort = "CUSTOMER_PHONE_TOO_SHORT";
    public const string CustomerPhoneTooLong = "CUSTOMER_PHONE_TOO_LONG";
    public const string CustomerPhoneInvalid = "CUSTOMER_PHONE_INVALID";

    // Customer Email
    public const string CustomerEmailRequired = "CUSTOMER_EMAIL_REQUIRED";
    public const string CustomerEmailInvalid = "CUSTOMER_EMAIL_INVALID";
    public const string CustomerEmailTooLong = "CUSTOMER_EMAIL_TOO_LONG";
    public const string CustomerEmailAlreadyExists = "CUSTOMER_EMAIL_ALREADY_EXISTS";

    // Customer Password
    public const string CustomerPasswordRequired = "CUSTOMER_PASSWORD_REQUIRED";
    public const string CustomerPasswordTooShort = "CUSTOMER_PASSWORD_TOO_SHORT";

    // Customer Identification
    public const string CustomerIdentificationAlreadyExists = "CUSTOMER_IDENTIFICATION_ALREADY_EXISTS";

    // Customer Not Found
    public const string CustomerNotFound = "CUSTOMER_NOT_FOUND";
}