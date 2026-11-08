using JustUrban.Data;
using JustUrban.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace JustUrban.Pages
{
    public class BillingModel : PageModel
    {
        private readonly AppDbContext _db;

        public BillingModel(AppDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        [Required(ErrorMessage = "Full Name is required")]
        public string CustomerName { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Contact Number is required")]
        public string ContactNumber { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "City is required")]
        public string City { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Pincode is required")]
        public string Pincode { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Street Address is required")]
        public string StreetAddress { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            BookingFlow.KeepBookingTempData(TempData);

            if (!AuthSession.IsLoggedIn(Request))
            {
                TempData["OpenLogin"] = "true";
                TempData["LoginError"] = "Please log in with your phone number before entering billing details.";
                return RedirectToPage("/Index");
            }

            var userId = AuthSession.UserId(Request);
            if (userId != null)
            {
                var user = await _db.Users.FindAsync(userId.Value);
                if (user != null)
                {
                    ContactNumber = user.PhoneDisplay;
                    CustomerName = user.FullName ?? string.Empty;
                    City = user.City ?? string.Empty;
                    Pincode = user.Pincode ?? string.Empty;
                    StreetAddress = user.StreetAddress ?? string.Empty;
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userId = AuthSession.UserId(Request);

            // Storing address and user ID in TempData for payment step
            TempData["UserId"] = userId;
            TempData["CustomerName"] = CustomerName;
            TempData["ContactNumber"] = ContactNumber;
            TempData["City"] = City;
            TempData["Pincode"] = Pincode;
            TempData["StreetAddress"] = StreetAddress;
            TempData["AmountDue"] = "49";

            BookingFlow.KeepBookingTempData(TempData);

            if (userId != null)
            {
                var user = await _db.Users.FindAsync(userId.Value);
                if (user != null)
                {
                    user.FullName = CustomerName;
                    user.PhoneDisplay = ContactNumber;
                    user.PhoneNumber = AuthSession.NormalizePhone(ContactNumber);
                    user.City = City;
                    user.Pincode = Pincode;
                    user.StreetAddress = StreetAddress;
                    user.UpdatedAt = DateTime.UtcNow;

                    await _db.SaveChangesAsync();
                    AuthSession.SignIn(Request, Response, user);
                }
            }

            return RedirectToPage("/payment");
        }
    }
}