using JustUrban.Models;
using Microsoft.EntityFrameworkCore;

namespace JustUrban.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<AppUser> Users => Set<AppUser>();
        public DbSet<Vendor> Vendors => Set<Vendor>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppUser>()
                .HasIndex(u => u.PhoneNumber);

            modelBuilder.Entity<AppUser>()
                .HasIndex(u => u.Email);

            modelBuilder.Entity<Vendor>()
                .HasOne(v => v.User)
                .WithOne(u => u.VendorProfile)
                .HasForeignKey<Vendor>(v => v.UserId);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Vendor)
                .WithMany(v => v.Bookings)
                .HasForeignKey(b => b.VendorId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
