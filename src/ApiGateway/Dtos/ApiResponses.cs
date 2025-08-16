namespace ApiGateway.Dtos;

// Respuestas base del Gateway
public record Response(
    string Message = "Operacion realizada con exito"
);

// Entidades para respuestas del Gateway
public record CustomerEntity(
    Guid Id,
    string Nombre,
    string Correo,
    string Telefono,
    string Direccion,
    string Identificacion,
    string Genero,
    int Edad,
    DateTime FechaCreacion
);

public record AccountEntity(
    Guid Id,
    Guid IdCliente,
    string NumeroCuenta,
    string TipoCuenta,
    decimal Saldo,
    bool EstaActiva,
    DateTime FechaCreacion
);

public record MovementEntity(
    Guid Id,
    Guid IdCuenta,
    string Tipo,
    decimal Monto,
    string Concepto,
    decimal SaldoAnterior,
    decimal SaldoNuevo,
    DateTime FechaCreacion
);

public record AuthEntity(
    string Correo,
    string Token,
    DateTime FechaExpiracion,
    Guid IdCliente
);

// Respuestas específicas del Gateway con datos
public record Customer(
    string Message,
    CustomerEntity Cliente
) : Response(Message);

public record Account(
    string Message,
    AccountEntity Cuenta
) : Response(Message);

public record Login(
    string Message,
    AuthEntity Auth
) : Response(Message);

public record CustomersList(
    string Message,
    CustomerEntity[] Clientes
) : Response(Message);

public record AccountsList(
    string Message,
    AccountEntity[] Cuentas
) : Response(Message);

public record MovementsList(
    string Message,
    MovementEntity[] Movimientos
) : Response(Message);


public record MovementReport(
    string Message,
    CustomerEntity Cliente,
    MovementEntity[] Movimientos,
    decimal TotalDepositos,
    decimal TotalRetiros,
    decimal MontoNeto
) : Response(Message);