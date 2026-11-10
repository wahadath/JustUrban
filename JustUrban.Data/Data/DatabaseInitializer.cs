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
            await AddColumnIfMissingAsync(db, "Users", "Role", "[Role] nvarchar(20) NOT NULL CONSTRAINT DF_Users_Role DEFAULT ('Customer')");
            await AddColumnIfMissingAsync(db, "Users", "Email", "Email nvarchar(200) NULL");
            await AddColumnIfMissingAsync(db, "Users", "PasswordHash", "PasswordHash nvarchar(500) NULL");
            await AddColumnIfMissingAsync(db, "Users", "IsActive", "IsActive bit NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT (1)");

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

            await AddColumnIfMissingAsync(db, "Vendors", "Pincode", "Pincode nvarchar(10) NULL");
            await AddColumnIfMissingAsync(db, "Vendors", "AadhaarNumber", "AadhaarNumber nvarchar(12) NULL");
            await AddColumnIfMissingAsync(db, "Vendors", "AadhaarPhotoPath", "AadhaarPhotoPath nvarchar(500) NULL");
            await AddColumnIfMissingAsync(db, "Vendors", "AadhaarBackPhotoPath", "AadhaarBackPhotoPath nvarchar(500) NULL");
            await AddColumnIfMissingAsync(db, "Vendors", "PanNumber", "PanNumber nvarchar(10) NULL");
            await AddColumnIfMissingAsync(db, "Vendors", "PanPhotoPath", "PanPhotoPath nvarchar(500) NULL");
            await AddColumnIfMissingAsync(db, "Vendors", "VerificationStatus", "VerificationStatus nvarchar(20) NOT NULL CONSTRAINT DF_Vendors_VerificationStatus DEFAULT ('Approved')");
            await AddColumnIfMissingAsync(db, "Vendors", "RejectionReason", "RejectionReason nvarchar(500) NULL");
            await AddColumnIfMissingAsync(db, "Vendors", "VerifiedAt", "VerifiedAt datetime2 NULL");

            await AddColumnIfMissingAsync(db, "Bookings", "UserId", "UserId int NULL");
            await AddColumnIfMissingAsync(db, "Bookings", "PartnerName", "PartnerName nvarchar(200) NULL");
            await AddColumnIfMissingAsync(db, "Bookings", "PartnerPhone", "PartnerPhone nvarchar(30) NULL");
            await AddColumnIfMissingAsync(db, "Bookings", "CancellationReason", "CancellationReason nvarchar(500) NULL");
            await AddColumnIfMissingAsync(db, "Bookings", "CancelledAt", "CancelledAt datetime2 NULL");
            await AddColumnIfMissingAsync(db, "Bookings", "VendorId", "VendorId int NULL");

            await db.Database.ExecuteSqlRawAsync("""
                IF OBJECT_ID('dbo.Bookings', 'U') IS NOT NULL
                   AND OBJECT_ID('dbo.Users', 'U') IS NOT NULL
                   AND COL_LENGTH('dbo.Bookings', 'UserId') IS NOT NULL
                   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Bookings_Users')
                    EXEC(N'ALTER TABLE dbo.Bookings ADD CONSTRAINT FK_Bookings_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id)');
                """);

            await db.Database.ExecuteSqlRawAsync("""
                IF OBJECT_ID('dbo.Bookings', 'U') IS NOT NULL
                   AND OBJECT_ID('dbo.Vendors', 'U') IS NOT NULL
                   AND COL_LENGTH('dbo.Bookings', 'VendorId') IS NOT NULL
                   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Bookings_Vendors')
                    EXEC(N'ALTER TABLE dbo.Bookings ADD CONSTRAINT FK_Bookings_Vendors FOREIGN KEY (VendorId) REFERENCES dbo.Vendors(Id)');
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
                    Pincode = "560001",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                db.Users.Add(vendorUser);
                await db.SaveChangesAsync();

                db.Vendors.Add(new Vendor
                {
                    UserId = vendorUser.Id,
                    BusinessName = vendorUser.FullName,
                    Phone = vendorUser.PhoneDisplay,
                    City = "Bangalore",
                    Pincode = "560001",
                    Services = "Painting, Polishing, Texture, Waterproofing",
                    IsAvailable = true,
                    VerificationStatus = VendorVerification.Approved,
                    VerifiedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                });
                await db.SaveChangesAsync();
            }
        }

        private static async Task AddColumnIfMissingAsync(AppDbContext db, string table, string column, string addSql)
        {
            // EXEC so SQL Server compiles ALTER only when the column is missing.
            var alter = $"ALTER TABLE dbo.{table} ADD {addSql}".Replace("'", "''");
            await db.Database.ExecuteSqlRawAsync($"""
                IF OBJECT_ID(N'dbo.{table}', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.{table}', N'{column}') IS NULL
                    EXEC(N'{alter}');
                """);
        }
    }
}
