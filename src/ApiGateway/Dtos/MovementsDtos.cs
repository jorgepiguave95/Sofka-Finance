namespace ApiGateway.Dtos;

public record ReportRequest(
    DateTime FechaInicio,
    DateTime FechaFin,
    Guid IdCliente
);
