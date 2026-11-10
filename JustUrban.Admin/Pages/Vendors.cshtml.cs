using JustUrban.Auth;
using JustUrban.Data;
using JustUrban.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace JustUrban.Admin.Pages
{
    public class VendorsModel : PageModel
    {
        private readonly AppDbContext _db;

        public VendorsModel(AppDbContext db)
        {
            _db = db;
        }

        public List<Vendor> PendingVendors { get; set; } = new();
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

        public async Task<IActionResult> OnPostApproveAsync(int id)
        {
            return await SetVerificationAsync(id, VendorVerification.Approved, null);
        }

        public async Task<IActionResult> OnPostRejectAsync(int id, string? rejectionReason)
        {
            return await SetVerificationAsync(id, VendorVerification.Rejected, rejectionReason);
        }

        private async Task<IActionResult> SetVerificationAsync(int id, string status, string? rejectionReason)
        {
            if (!StaffSession.IsLoggedIn(Request, StaffSession.AdminApp))
            {
                return RedirectToPage("/Login");
            }

            var vendor = await _db.Vendors.Include(v => v.User).FirstOrDefaultAsync(v => v.Id == id);
            if (vendor != null)
            {
                vendor.VerificationStatus = status;
                vendor.VerifiedAt = DateTime.UtcNow;
                vendor.IsAvailable = status == VendorVerification.Approved;
                vendor.RejectionReason = status == VendorVerification.Rejected
                    ? (string.IsNullOrWhiteSpace(rejectionReason) ? "Admin selected No." : rejectionReason.Trim())
                    : null;
                await _db.SaveChangesAsync();
                var name = string.IsNullOrWhiteSpace(vendor.User?.FullName) ? vendor.BusinessName : vendor.User!.FullName;
                Message = status == VendorVerification.Approved
                    ? $"{name} approved. They can now take jobs."
                    : $"{name} rejected.";
            }

            await LoadAsync();
            return Page();
        }

        private async Task LoadAsync()
        {
            var all = await _db.Vendors.Include(v => v.User).OrderBy(v => v.User!.FullName).ToListAsync();
            PendingVendors = all.Where(v => v.VerificationStatus == VendorVerification.Pending).ToList();
            Vendors = all;
        }
    }
}
