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

        public List<Vendor> Vendors { get; set; } = new();

        [BindProperty]
        public string BusinessName { get; set; } = string.Empty;

        [BindProperty]
        public string Phone { get; set; } = string.Empty;

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = "Vendor@123";

        [BindProperty]
        public string? City { get; set; }

        [BindProperty]
        public string? Services { get; set; }

        public string? Message { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!StaffSession.IsLoggedIn(Request, StaffSession.AdminApp))
            {
                return RedirectToPage("/Login");
            }

            await LoadAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!StaffSession.IsLoggedIn(Request, StaffSession.AdminApp))
            {
                return RedirectToPage("/Login");
            }

            if (string.IsNullOrWhiteSpace(BusinessName) || string.IsNullOrWhiteSpace(Phone) || string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Business name, phone and email are required.";
                await LoadAsync();
                return Page();
            }

            var digits = new string(Phone.Where(char.IsDigit).ToArray());
            var user = new AppUser
            {
                PhoneNumber = digits,
                PhoneDisplay = Phone.Trim(),
                FullName = BusinessName.Trim(),
                Email = Email.Trim(),
                PasswordHash = PasswordHelper.Hash(string.IsNullOrWhiteSpace(Password) ? "Vendor@123" : Password),
                Role = UserRoles.Vendor,
                City = City,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            _db.Vendors.Add(new Vendor
            {
                UserId = user.Id,
                BusinessName = BusinessName.Trim(),
                Phone = Phone.Trim(),
                City = City,
                Services = Services,
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();
            Message = "Vendor created. They can log in on the Vendor app.";
            await LoadAsync();
            return Page();
        }

        private async Task LoadAsync()
        {
            Vendors = await _db.Vendors.Include(v => v.User).OrderBy(v => v.BusinessName).ToListAsync();
        }
    }
}
