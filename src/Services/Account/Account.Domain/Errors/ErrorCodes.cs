namespace Account.Domain.Errors;

public static class ErrorCodes
{
    // Account errors
    public const string ACCOUNT_NUMBER_REQUIRED = "ACCOUNT_NUMBER_REQUIRED";
    public const string ACCOUNT_NUMBER_INVALID = "ACCOUNT_NUMBER_INVALID";
    public const string ACCOUNT_NUMBER_TOO_SHORT = "ACCOUNT_NUMBER_TOO_SHORT";
    public const string ACCOUNT_NUMBER_TOO_LONG = "ACCOUNT_NUMBER_TOO_LONG";
    public const string ACCOUNT_INACTIVE = "ACCOUNT_INACTIVE";
    public const string ACCOUNT_ALREADY_INACTIVE = "ACCOUNT_ALREADY_INACTIVE";
    public const string ACCOUNT_HAS_BALANCE = "ACCOUNT_HAS_BALANCE";
    public const string ACCOUNT_NOT_FOUND = "ACCOUNT_NOT_FOUND";
    public const string ACCOUNT_ID_REQUIRED = "ACCOUNT_ID_REQUIRED";
    public const string ACCOUNT_TYPE_REQUIRED = "ACCOUNT_TYPE_REQUIRED";
    public const string ACCOUNT_TYPE_INVALID = "ACCOUNT_TYPE_INVALID";
    public const string ACCOUNT_NUMBER_ALREADY_EXISTS = "ACCOUNT_NUMBER_ALREADY_EXISTS";

    public const string MOVEMENT_TYPE_REQUIRED = "MOVEMENT_TYPE_REQUIRED";
    public const string MOVEMENT_TYPE_INVALID = "MOVEMENT_TYPE_INVALID";

    // Customer errors
    public const string CUSTOMER_ID_REQUIRED = "CUSTOMER_ID_REQUIRED";

    // Balance errors
    public const string INITIAL_BALANCE_INVALID = "INITIAL_BALANCE_INVALID";
    public const string INSUFFICIENT_BALANCE = "INSUFFICIENT_BALANCE";

    // Movement errors
    public const string MOVEMENT_VALUE_ZERO = "MOVEMENT_VALUE_ZERO";
    public const string AMOUNT_INVALID = "AMOUNT_INVALID";
    public const string AMOUNT_DECIMAL_INVALID = "AMOUNT_DECIMAL_INVALID";

    // Transfer errors
    public const string TARGET_ACCOUNT_REQUIRED = "TARGET_ACCOUNT_REQUIRED";
    public const string TARGET_ACCOUNT_INACTIVE = "TARGET_ACCOUNT_INACTIVE";
    public const string SAME_ACCOUNT_TRANSFER = "SAME_ACCOUNT_TRANSFER";
}