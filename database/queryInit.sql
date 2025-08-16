---------------------------------------
-- Crear base CustomerSofka
---------------------------------------
IF DB_ID(N'CustomerSofka') IS NULL
BEGIN
    PRINT 'Creando base de datos [CustomerSofka]...';
    CREATE DATABASE CustomerSofka;
END
GO

USE CustomerSofka;
GO

-- Tabla Clientes
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'clientes' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.clientes (
        idCliente          UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        nombre             VARCHAR(150)     NOT NULL,
        genero             VARCHAR(20)      NULL,
        edad               INT              NULL,
        identificacion     VARCHAR(50)      NOT NULL UNIQUE,
        direccion          VARCHAR(200)     NULL,
        telefono           VARCHAR(30)      NULL,
        contraseña         VARCHAR(200)     NOT NULL,
        activo             BIT              NOT NULL DEFAULT 1,
        fechaCreacion      DATETIME2(0)     NOT NULL DEFAULT SYSDATETIME(),
        fechaActualizacion DATETIME2(0)     NULL
    );
END
GO


---------------------------------------
-- Crear base AccountSofka
---------------------------------------
IF DB_ID(N'AccountSofka') IS NULL
BEGIN
    PRINT 'Creando base de datos [AccountSofka]...';
    CREATE DATABASE AccountSofka;
END
GO

USE AccountSofka;
GO

-- Tabla cuentas
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'cuentas' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.cuentas (
        idCuenta           UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        idCliente          UNIQUEIDENTIFIER NOT NULL,
        numero             VARCHAR(30)      NOT NULL UNIQUE,
        tipo               VARCHAR(30)      NOT NULL,
        saldo              DECIMAL(10,2)    NOT NULL DEFAULT 0,
        activo             BIT              NOT NULL DEFAULT 1,
        fechaCreacion      DATETIME2(0)     NOT NULL DEFAULT SYSDATETIME(),
        fechaActualizacion DATETIME2(0)     NULL
    );
END
GO

-- Tabla movimientos
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'movimientos' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.movimientos (
        idMovimiento    UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        idCuenta        UNIQUEIDENTIFIER NOT NULL,
        tipo            VARCHAR(20)      NOT NULL,
        valor           DECIMAL(10,2)    NOT NULL,
        saldoDisponible DECIMAL(10,2)    NOT NULL,
        estado          BIT              NOT NULL DEFAULT 1,
        fecha           DATETIME2(0)     NOT NULL DEFAULT SYSDATETIME()
    );
END
GO
