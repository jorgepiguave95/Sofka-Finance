namespace Account.Domain.Errors;

public static class ErrorCodes
{
    public const string ACCOUNT_NUMBER_REQUIRED = "ACCOUNT_NUMBER_REQUIRED";
    public const string INITIAL_BALANCE_INVALID = "INITIAL_BALANCE_INVALID";
    public const string ACCOUNT_INACTIVE = "ACCOUNT_INACTIVE";
    public const string INSUFFICIENT_BALANCE = "INSUFFICIENT_BALANCE";
    public const string MOVEMENT_VALUE_ZERO = "MOVEMENT_VALUE_ZERO";
}
