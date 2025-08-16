namespace ApiGateway.Dtos;

public record OperationResponse(
    Guid OperationId,
    string Message = "Operation accepted and will be processed",
    int StatusCode = 202
);

public record CustomerOperationResponse(
    Guid OperationId,
    string Operation,
    string Message = "Customer operation accepted and will be processed",
    int StatusCode = 202
) : OperationResponse(OperationId, Message, StatusCode);

public record MovementOperationResponse(
    Guid OperationId,
    string Operation,
    Guid? AccountId = null,
    string Message = "Movement operation accepted and will be processed",
    int StatusCode = 202
) : OperationResponse(OperationId, Message, StatusCode);

public record AuthOperationResponse(
    Guid OperationId,
    string Operation,
    string Message = "Authentication operation accepted and will be processed",
    int StatusCode = 202
) : OperationResponse(OperationId, Message, StatusCode);

public record AccountOperationResponse(
    Guid OperationId,
    string Operation,
    string Message = "Account operation accepted and will be processed",
    int StatusCode = 202
) : OperationResponse(OperationId, Message, StatusCode);
