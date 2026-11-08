using JustUrban.Data;
using JustUrban.Models;
using JustUrban.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace JustUrban.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _db;

        public IndexModel(AppDbContext db)
        {
            _db = db;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostVerifyOtp(string FullPhoneNumber, string OtpCode)
        {
            if (OtpCode != "1234")
            {
                TempData["OpenLogin"] = "true";
                TempData["LoginError"] = "Invalid OTP. Use 1234 for this demo.";
                return RedirectToPage("/Index");
            }

            var normalized = AuthSession.NormalizePhone(FullPhoneNumber);
            if (normalized.Length < 10)
            {
                TempData["OpenLogin"] = "true";
                TempData["LoginError"] = "Enter a valid 10-digit phone number.";
                return RedirectToPage("/Index");
            }

            var user = await _db.Users.FirstOrDefaultAsync(u => u.PhoneNumber == normalized);
            if (user == null)
            {
                user = new AppUser
                {
                    PhoneNumber = normalized,
                    PhoneDisplay = FullPhoneNumber.Trim(),
                    CreatedAt = DateTime.UtcNow
                };
                _db.Users.Add(user);
                await _db.SaveChangesAsync();
            }
            else
            {
                user.PhoneDisplay = FullPhoneNumber.Trim();
                await _db.SaveChangesAsync();
            }

            AuthSession.SignIn(Request, Response, user);
            BookingFlow.KeepBookingTempData(TempData);

            if (TempData.ContainsKey("UserPickedDate"))
            {
                return RedirectToPage("/Billing");
            }

            return RedirectToPage("/Profile");
        }
    }
}
