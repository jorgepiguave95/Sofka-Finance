---------------------------------------
-- Crear base CustomerSofka
---------------------------------------
IF DB_ID(N'CustomerSofka') IS NULL
BEGIN
    CREATE DATABASE CustomerSofka;
END
GO

USE CustomerSofka;
GO

-- Tabla Clients
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Clients' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Clients (
        Id                 UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        Name               NVARCHAR(100)    NOT NULL,
        Gender             NVARCHAR(10)     NOT NULL,
        Age                INT              NOT NULL,
        Identification     NVARCHAR(20)     NOT NULL,
        Address            NVARCHAR(200)    NOT NULL,
        Phone              NVARCHAR(15)     NOT NULL,
        Email              NVARCHAR(100)    NOT NULL,
        PasswordHash       NVARCHAR(255)    NOT NULL,
        IsActive           BIT              NOT NULL,
        CreatedAt          DATETIME2        NOT NULL,
        UpdatedAt          DATETIME2        NOT NULL
    );

    CREATE UNIQUE INDEX IX_Clients_Email ON dbo.Clients (Email);
    CREATE UNIQUE INDEX IX_Clients_Identification ON dbo.Clients (Identification);
END
GO


---------------------------------------
-- Crear base AccountSofka
---------------------------------------
IF DB_ID(N'AccountSofka') IS NULL
BEGIN
    CREATE DATABASE AccountSofka;
END
GO

USE AccountSofka;
GO

-- Tabla Accounts
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Accounts' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Accounts (
        Id             UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        CustomerId     UNIQUEIDENTIFIER NOT NULL,
        AccountNumber  NVARCHAR(30)     NOT NULL,
        AccountType    NVARCHAR(20)     NOT NULL,
        Balance        DECIMAL(18,2)    NOT NULL,
        IsActive       BIT              NOT NULL DEFAULT 1,
        CreatedAt      DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt      DATETIME2        NULL
    );

    CREATE UNIQUE INDEX IX_Accounts_AccountNumber ON dbo.Accounts (AccountNumber);
END
GO

-- Tabla Movements (basada en EF Core MovementConfiguration)
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Movements' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Movements (
        Id               UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        AccountId        UNIQUEIDENTIFIER NOT NULL,
        MovementType     NVARCHAR(20)     NOT NULL,
        Value            DECIMAL(18,2)    NOT NULL,
        AvailableBalance DECIMAL(18,2)    NOT NULL,
        Status           BIT              NOT NULL DEFAULT 1,
        Date             DATETIME2        NOT NULL,
        Concept          NVARCHAR(200)    NULL,
        
        CONSTRAINT FK_Movements_Accounts_AccountId 
            FOREIGN KEY (AccountId) REFERENCES dbo.Accounts(Id) ON DELETE NO ACTION
    );

    CREATE INDEX IX_Movements_AccountId ON dbo.Movements (AccountId);
END
GO
