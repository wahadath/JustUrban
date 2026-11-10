using JustUrban.Auth;
using JustUrban.Data;
using JustUrban.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace JustUrban.Admin.Pages
{
    public class BookingsModel : PageModel
    {
        private readonly AppDbContext _db;

        public BookingsModel(AppDbContext db)
        {
            _db = db;
        }

        public List<Booking> Bookings { get; set; } = new();
        public List<Vendor> Vendors { get; set; } = new();
        public string? Message { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!StaffSession.IsLoggedIn(Request, StaffSession.AdminApp))
            {
                return RedirectToPage("/Login");
            }

            await LoadAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAssignAsync(int bookingId, int vendorId)
        {
            if (!StaffSession.IsLoggedIn(Request, StaffSession.AdminApp))
            {
                return RedirectToPage("/Login");
            }

            var booking = await _db.Bookings.FindAsync(bookingId);
            var vendor = await _db.Vendors.Include(v => v.User).FirstOrDefaultAsync(v => v.Id == vendorId);
            if (booking != null && vendor != null && vendor.IsApproved && booking.Status != "Cancelled")
            {
                var name = string.IsNullOrWhiteSpace(vendor.User?.FullName) ? vendor.BusinessName : vendor.User!.FullName;
                booking.VendorId = vendor.Id;
                booking.PartnerName = name;
                booking.PartnerPhone = vendor.Phone;
                booking.Status = "Partner Assigned";
                booking.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
                Message = $"Assigned {name} to booking #{booking.Id}.";
            }

            await LoadAsync();
            return Page();
        }

        private async Task LoadAsync()
        {
            Bookings = await _db.Bookings
                .Include(b => b.Vendor)
                    .ThenInclude(v => v!.User)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
            Vendors = await _db.Vendors
                .Include(v => v.User)
                .Where(v => v.IsAvailable && v.VerificationStatus == VendorVerification.Approved)
                .OrderBy(v => v.User!.FullName)
                .ToListAsync();
        }
    }
}
