using JustUrban.Auth;
using JustUrban.Models;
using Microsoft.EntityFrameworkCore;

namespace JustUrban.Data
{
    public static class DatabaseInitializer
    {
        public static async Task EnsureAsync(AppDbContext db)
        {
            await db.Database.EnsureCreatedAsync();
            await ApplySchemaUpdatesAsync(db);
            await SeedStaffAsync(db);
        }

        private static async Task ApplySchemaUpdatesAsync(AppDbContext db)
        {
            await db.Database.ExecuteSqlRawAsync("""
                IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL
                BEGIN
                    IF COL_LENGTH('dbo.Users', 'Role') IS NULL
                        ALTER TABLE dbo.Users ADD [Role] nvarchar(20) NOT NULL CONSTRAINT DF_Users_Role DEFAULT ('Customer');
                    IF COL_LENGTH('dbo.Users', 'Email') IS NULL
                        ALTER TABLE dbo.Users ADD Email nvarchar(200) NULL;
                    IF COL_LENGTH('dbo.Users', 'PasswordHash') IS NULL
                        ALTER TABLE dbo.Users ADD PasswordHash nvarchar(500) NULL;
                    IF COL_LENGTH('dbo.Users', 'IsActive') IS NULL
                        ALTER TABLE dbo.Users ADD IsActive bit NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT (1);
                END
                """);

            await db.Database.ExecuteSqlRawAsync("""
                IF OBJECT_ID('dbo.Vendors', 'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.Vendors (
                        Id int IDENTITY(1,1) NOT NULL PRIMARY KEY,
                        UserId int NOT NULL,
                        BusinessName nvarchar(200) NOT NULL,
                        Phone nvarchar(30) NOT NULL,
                        City nvarchar(100) NULL,
                        Services nvarchar(200) NULL,
                        IsAvailable bit NOT NULL CONSTRAINT DF_Vendors_IsAvailable DEFAULT (1),
                        CreatedAt datetime2 NOT NULL,
                        CONSTRAINT FK_Vendors_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id)
                    );
                END
                """);

            await db.Database.ExecuteSqlRawAsync("""
                IF OBJECT_ID('dbo.Bookings', 'U') IS NOT NULL
                BEGIN
                    IF COL_LENGTH('dbo.Bookings', 'UserId') IS NULL
                        ALTER TABLE dbo.Bookings ADD UserId int NULL;
                    IF COL_LENGTH('dbo.Bookings', 'PartnerName') IS NULL
                        ALTER TABLE dbo.Bookings ADD PartnerName nvarchar(200) NULL;
                    IF COL_LENGTH('dbo.Bookings', 'PartnerPhone') IS NULL
                        ALTER TABLE dbo.Bookings ADD PartnerPhone nvarchar(30) NULL;
                    IF COL_LENGTH('dbo.Bookings', 'CancellationReason') IS NULL
                        ALTER TABLE dbo.Bookings ADD CancellationReason nvarchar(500) NULL;
                    IF COL_LENGTH('dbo.Bookings', 'CancelledAt') IS NULL
                        ALTER TABLE dbo.Bookings ADD CancelledAt datetime2 NULL;
                    IF COL_LENGTH('dbo.Bookings', 'VendorId') IS NULL
                        ALTER TABLE dbo.Bookings ADD VendorId int NULL;
                    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Bookings_Users')
                       AND COL_LENGTH('dbo.Bookings', 'UserId') IS NOT NULL
                       AND OBJECT_ID('dbo.Users', 'U') IS NOT NULL
                    BEGIN
                        ALTER TABLE dbo.Bookings ADD CONSTRAINT FK_Bookings_Users
                            FOREIGN KEY (UserId) REFERENCES dbo.Users(Id);
                    END
                    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Bookings_Vendors')
                       AND COL_LENGTH('dbo.Bookings', 'VendorId') IS NOT NULL
                       AND OBJECT_ID('dbo.Vendors', 'U') IS NOT NULL
                    BEGIN
                        ALTER TABLE dbo.Bookings ADD CONSTRAINT FK_Bookings_Vendors
                            FOREIGN KEY (VendorId) REFERENCES dbo.Vendors(Id);
                    END
                END
                """);
        }

        private static async Task SeedStaffAsync(AppDbContext db)
        {
            if (!await db.Users.AnyAsync(u => u.Role == UserRoles.Admin && u.Email == "admin@justurban.local"))
            {
                db.Users.Add(new AppUser
                {
                    PhoneNumber = "9999999999",
                    PhoneDisplay = "+91 9999999999",
                    FullName = "Just Urban Admin",
                    Email = "admin@justurban.local",
                    PasswordHash = PasswordHelper.Hash("Admin@123"),
                    Role = UserRoles.Admin,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
                await db.SaveChangesAsync();
            }

            if (!await db.Users.AnyAsync(u => u.Role == UserRoles.Vendor && u.Email == "vendor@justurban.local"))
            {
                var vendorUser = new AppUser
                {
                    PhoneNumber = "8888888888",
                    PhoneDisplay = "+91 8888888888",
                    FullName = "Demo Painter",
                    Email = "vendor@justurban.local",
                    PasswordHash = PasswordHelper.Hash("Vendor@123"),
                    Role = UserRoles.Vendor,
                    City = "Bangalore",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                db.Users.Add(vendorUser);
                await db.SaveChangesAsync();

                db.Vendors.Add(new Vendor
                {
                    UserId = vendorUser.Id,
                    BusinessName = "Just Urban Demo Crew",
                    Phone = vendorUser.PhoneDisplay,
                    City = "Bangalore",
                    Services = "Painting, Polishing, Texture, Waterproofing",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow
                });
                await db.SaveChangesAsync();
            }
        }
    }
}
