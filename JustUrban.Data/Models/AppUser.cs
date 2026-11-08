using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JustUrban.Models
{
    [Table("Users")]
    public class AppUser
    {
        public int Id { get; set; }

        [Required, MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required, MaxLength(40)]
        public string PhoneDisplay { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? FullName { get; set; }

        [MaxLength(200)]
        public string? Email { get; set; }

        [MaxLength(500)]
        public string? PasswordHash { get; set; }

        [Required, MaxLength(20)]
        public string Role { get; set; } = UserRoles.Customer;

        public bool IsActive { get; set; } = true;

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(10)]
        public string? Pincode { get; set; }

        [MaxLength(500)]
        public string? StreetAddress { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public virtual Vendor? VendorProfile { get; set; }

        public string NavbarLabel =>
            string.IsNullOrWhiteSpace(FullName) ? "Account" : FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];

        public bool HasAddress =>
            !string.IsNullOrWhiteSpace(StreetAddress) || !string.IsNullOrWhiteSpace(City);
    }
}
