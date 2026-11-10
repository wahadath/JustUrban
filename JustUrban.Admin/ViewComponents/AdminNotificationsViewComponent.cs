using JustUrban.Data;
using JustUrban.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JustUrban.Admin.ViewComponents
{
    public class AdminNotificationsViewComponent : ViewComponent
    {
        private readonly AppDbContext _db;

        public AdminNotificationsViewComponent(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var pendingVendors = await _db.Vendors
                .Where(v => v.VerificationStatus == VendorVerification.Pending)
                .OrderByDescending(v => v.CreatedAt)
                .Take(8)
                .Select(v => new AdminNotificationItem
                {
                    Kind = "Vendor",
                    Title = (v.User != null && v.User.FullName != "" ? v.User.FullName : v.BusinessName) + " is waiting for verification",
                    Detail = (v.City ?? "") + (string.IsNullOrWhiteSpace(v.Pincode) ? "" : " " + v.Pincode),
                    Href = "/Vendors",
                    CreatedAt = v.CreatedAt
                })
                .ToListAsync();

            var openBookings = await _db.Bookings
                .Where(b => b.VendorId == null && b.Status != "Cancelled" && b.Status != "Completed")
                .OrderByDescending(b => b.CreatedAt)
                .Take(8)
                .Select(b => new AdminNotificationItem
                {
                    Kind = "Booking",
                    Title = "Booking #" + b.Id + " needs a vendor",
                    Detail = b.ServiceName + " · " + b.CustomerName,
                    Href = "/Bookings",
                    CreatedAt = b.CreatedAt
                })
                .ToListAsync();

            var vendorCount = await _db.Vendors.CountAsync(v => v.VerificationStatus == VendorVerification.Pending);
            var bookingCount = await _db.Bookings.CountAsync(b =>
                b.VendorId == null && b.Status != "Cancelled" && b.Status != "Completed");

            var model = new AdminNotificationList
            {
                Count = vendorCount + bookingCount,
                Items = pendingVendors
                    .Concat(openBookings)
                    .OrderByDescending(i => i.CreatedAt)
                    .Take(10)
                    .ToList()
            };

            return View(model);
        }
    }

    public class AdminNotificationList
    {
        public int Count { get; set; }
        public List<AdminNotificationItem> Items { get; set; } = new();
    }

    public class AdminNotificationItem
    {
        public string Kind { get; set; } = "Booking";
        public string Title { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
        public string Href { get; set; } = "/";
        public DateTime CreatedAt { get; set; }

        public string When
        {
            get
            {
                var ago = DateTime.UtcNow - CreatedAt;
                if (ago.TotalMinutes < 1) return "Just now";
                if (ago.TotalHours < 1) return $"{(int)ago.TotalMinutes}m ago";
                if (ago.TotalDays < 1) return $"{(int)ago.TotalHours}h ago";
                if (ago.TotalDays < 7) return $"{(int)ago.TotalDays}d ago";
                return CreatedAt.ToLocalTime().ToString("dd MMM");
            }
        }
    }
}
