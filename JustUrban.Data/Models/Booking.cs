using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JustUrban.Models
{
    [Table("Bookings")]
    public class Booking
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual AppUser? User { get; set; }

        public int? VendorId { get; set; }

        [ForeignKey(nameof(VendorId))]
        public virtual Vendor? Vendor { get; set; }

        [Required, MaxLength(200)]
        public string ServiceName { get; set; } = "Home Makeover";

        [Required, MaxLength(200)]
        public string CustomerName { get; set; } = string.Empty;

        [Required, MaxLength(30)]
        public string ContactNumber { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(10)]
        public string? Pincode { get; set; }

        [MaxLength(500)]
        public string? StreetAddress { get; set; }

        public DateTime SlotDate { get; set; }

        [Required, MaxLength(20)]
        public string SlotTime { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string PaymentMethod { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; } = 49.00m;

        [Required, MaxLength(30)]
        public string Status { get; set; } = "Confirmed";

        [MaxLength(200)]
        public string? PartnerName { get; set; }

        [MaxLength(30)]
        public string? PartnerPhone { get; set; }

        [MaxLength(500)]
        public string? CancellationReason { get; set; }

        public DateTime? CancelledAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool CanReschedule => Status is "Confirmed" or "Partner Assigned" or "Assigned";

        public bool CanCancel => Status is "Confirmed" or "Partner Assigned" or "Assigned";

        public string FormattedSlot => $"{SlotDate:ddd, dd MMM yyyy} at {SlotTime}";

        public bool CanModify => Status != "Cancelled" && Status != "Completed" && SlotDate.Date >= DateTime.Today;
    }
}
