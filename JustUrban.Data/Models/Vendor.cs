using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JustUrban.Models
{
    [Table("Vendors")]
    public class Vendor
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual AppUser? User { get; set; }

        [Required, MaxLength(200)]
        public string BusinessName { get; set; } = string.Empty;

        [Required, MaxLength(30)]
        public string Phone { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(10)]
        public string? Pincode { get; set; }

        [MaxLength(200)]
        public string? Services { get; set; }

        public bool IsAvailable { get; set; } = true;

        [MaxLength(12)]
        public string? AadhaarNumber { get; set; }

        [MaxLength(500)]
        public string? AadhaarPhotoPath { get; set; }

        [MaxLength(500)]
        public string? AadhaarBackPhotoPath { get; set; }

        [MaxLength(10)]
        public string? PanNumber { get; set; }

        [MaxLength(500)]
        public string? PanPhotoPath { get; set; }

        [Required, MaxLength(20)]
        public string VerificationStatus { get; set; } = VendorVerification.Pending;

        [MaxLength(500)]
        public string? RejectionReason { get; set; }

        public DateTime? VerifiedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [NotMapped]
        public bool IsApproved => VerificationStatus == VendorVerification.Approved;

        [NotMapped]
        public bool IsPending => VerificationStatus == VendorVerification.Pending;

        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
