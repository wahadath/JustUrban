-- JustUrban: run this in Visual Studio / Azure Data Studio against LocalDB
-- Connect to: (localdb)\MSSQLLocalDB
-- Then Execute (Ctrl+Shift+E)

IF DB_ID(N'JustUrban') IS NULL
BEGIN
    CREATE DATABASE JustUrban;
END
GO

USE JustUrban;
GO

IF OBJECT_ID(N'dbo.Bookings', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Bookings
    (
        Id              INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        ServiceName     NVARCHAR(200) NOT NULL,
        CustomerName    NVARCHAR(200) NOT NULL,
        ContactNumber   NVARCHAR(30)  NOT NULL,
        City            NVARCHAR(100) NULL,
        Pincode         NVARCHAR(10)  NULL,
        StreetAddress   NVARCHAR(500) NULL,
        SlotDate        DATE          NOT NULL,
        SlotTime        NVARCHAR(20)  NOT NULL,
        PaymentMethod   NVARCHAR(20)  NOT NULL,
        Amount          DECIMAL(10,2) NOT NULL CONSTRAINT DF_Bookings_Amount DEFAULT (49.00),
        Status          NVARCHAR(30)  NOT NULL CONSTRAINT DF_Bookings_Status DEFAULT (N'Confirmed'),
        CreatedAt       DATETIME2     NOT NULL CONSTRAINT DF_Bookings_CreatedAt DEFAULT (SYSUTCDATETIME()),
        UpdatedAt       DATETIME2     NULL
    );

    CREATE INDEX IX_Bookings_ContactNumber ON dbo.Bookings (ContactNumber);
    CREATE INDEX IX_Bookings_Status ON dbo.Bookings (Status);
END
GO

IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        Id              INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        PhoneNumber     NVARCHAR(20)  NOT NULL UNIQUE,
        PhoneDisplay    NVARCHAR(40)  NOT NULL,
        FullName        NVARCHAR(200) NULL,
        City            NVARCHAR(100) NULL,
        Pincode         NVARCHAR(10)  NULL,
        StreetAddress   NVARCHAR(500) NULL,
        CreatedAt       DATETIME2     NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT (SYSUTCDATETIME()),
        UpdatedAt       DATETIME2     NULL
    );
END
GO

SELECT * FROM dbo.Bookings;
SELECT * FROM dbo.Users;
GO
