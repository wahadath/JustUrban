using JustUrban.Auth;
using JustUrban.Data;
using JustUrban.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace JustUrban.Vendor.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _db;

        public IndexModel(AppDbContext db)
        {
            _db = db;
        }

        public List<Booking> Jobs { get; set; } = new();
        public string? Message { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var redirect = await EnsureVendorAsync();
            if (redirect != null)
            {
                return redirect;
            }

            await LoadAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostStartAsync(int id)
        {
            return await UpdateStatusAsync(id, "In Progress");
        }

        public async Task<IActionResult> OnPostCompleteAsync(int id)
        {
            return await UpdateStatusAsync(id, "Completed");
        }

        private async Task<IActionResult> UpdateStatusAsync(int id, string status)
        {
            var redirect = await EnsureVendorAsync();
            if (redirect != null)
            {
                return redirect;
            }

            var vendorId = StaffSession.VendorId(Request);
            var booking = await _db.Bookings.FirstOrDefaultAsync(b => b.Id == id && b.VendorId == vendorId);
            if (booking != null && booking.Status != "Cancelled")
            {
                booking.Status = status;
                booking.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
                Message = $"Booking #{booking.Id} marked {status}.";
            }

            await LoadAsync();
            return Page();
        }

        private async Task LoadAsync()
        {
            var vendorId = StaffSession.VendorId(Request);
            Jobs = await _db.Bookings
                .Where(b => b.VendorId == vendorId)
                .OrderByDescending(b => b.SlotDate)
                .ToListAsync();
        }

        private Task<IActionResult?> EnsureVendorAsync()
        {
            if (!StaffSession.IsLoggedIn(Request, StaffSession.VendorApp) || StaffSession.VendorId(Request) == null)
            {
                return Task.FromResult<IActionResult?>(RedirectToPage("/Login"));
            }

            return Task.FromResult<IActionResult?>(null);
        }
    }
}
