using System.Text.RegularExpressions;
using JustUrban.Auth;
using JustUrban.Data;
using JustUrban.Models;
using JustUrban.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace JustUrban.Vendor.Pages
{
    [RequestFormLimits(MultipartBodyLengthLimit = 20 * 1024 * 1024)]
    [RequestSizeLimit(20 * 1024 * 1024)]
    public class RegisterModel : PageModel
    {
        private static readonly Regex AadhaarPattern = new(@"^\d{12}$", RegexOptions.Compiled);
        private static readonly Regex PanPattern = new(@"^[A-Z]{5}[0-9]{4}[A-Z]$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public RegisterModel(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        [BindProperty]
        public string FullName { get; set; } = string.Empty;

        [BindProperty]
        public string Phone { get; set; } = string.Empty;

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        public string? City { get; set; }

        [BindProperty]
        public string Pincode { get; set; } = string.Empty;

        [BindProperty]
        public string? Services { get; set; }

        [BindProperty]
        public string AadhaarNumber { get; set; } = string.Empty;

        // [BindProperty]
        // public string AadhaarOtp { get; set; } = string.Empty;

        [BindProperty]
        public IFormFile? AadhaarFrontPhoto { get; set; }

        [BindProperty]
        public IFormFile? AadhaarBackPhoto { get; set; }

        [BindProperty]
        public string PanNumber { get; set; } = string.Empty;

        [BindProperty]
        public IFormFile? PanPhoto { get; set; }

        [BindProperty]
        public bool NoPan { get; set; }

        public string? ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var aadhaar = new string(AadhaarNumber.Where(char.IsDigit).ToArray());
            var pan = string.IsNullOrWhiteSpace(PanNumber) ? null : PanNumber.Trim().ToUpperInvariant();
            var digits = new string(Phone.Where(char.IsDigit).ToArray());
            var pincode = new string(Pincode.Where(char.IsDigit).ToArray());
            var email = Email.Trim();

            if (string.IsNullOrWhiteSpace(FullName)
                || string.IsNullOrWhiteSpace(Phone) || string.IsNullOrWhiteSpace(email)
                || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Your name, phone, email and password are required.";
                return Page();
            }

            if (digits.Length < 10)
            {
                ErrorMessage = "Enter a valid 10-digit phone number.";
                return Page();
            }

            if (pincode.Length != 6)
            {
                ErrorMessage = "Zip code must be 6 digits.";
                return Page();
            }

            if (!AadhaarPattern.IsMatch(aadhaar))
            {
                ErrorMessage = "Aadhaar number must be 12 digits.";
                return Page();
            }

            // Aadhaar OTP is not enforced yet. Uncomment when AadhaarOtpClient is connected.
            // SendOtpAsync texts the OTP to the mobile number registered on this Aadhaar.
            // The vendor must enter that OTP before the account is created.
            //
            // var aadhaarTxnId = await AadhaarOtpClient.SendOtpAsync(aadhaar);
            // if (string.IsNullOrWhiteSpace(AadhaarOtp) || !await AadhaarOtpClient.VerifyOtpAsync(aadhaarTxnId, AadhaarOtp))
            // {
            //     ErrorMessage = "Enter the OTP sent to the mobile number linked with this Aadhaar.";
            //     return Page();
            // }

            if (!NoPan && (string.IsNullOrWhiteSpace(pan) || !PanPattern.IsMatch(pan)))
            {
                ErrorMessage = "PAN must look like ABCDE1234F.";
                return Page();
            }

            var aadhaarFrontError = KycStorage.Validate(AadhaarFrontPhoto);
            if (aadhaarFrontError != null)
            {
                ErrorMessage = "Aadhaar front photo: " + aadhaarFrontError;
                return Page();
            }

            var aadhaarBackError = KycStorage.Validate(AadhaarBackPhoto);
            if (aadhaarBackError != null)
            {
                ErrorMessage = "Aadhaar back photo: " + aadhaarBackError;
                return Page();
            }

            if (!NoPan)
            {
                var panError = KycStorage.Validate(PanPhoto);
                if (panError != null)
                {
                    ErrorMessage = "PAN photo: " + panError;
                    return Page();
                }
            }

            if (await _db.Users.AnyAsync(u => u.Email == email))
            {
                ErrorMessage = "This email is already registered.";
                return Page();
            }

            if (await _db.Users.AnyAsync(u => u.PhoneNumber == digits && u.Role == UserRoles.Vendor))
            {
                ErrorMessage = "This phone number is already registered as a vendor.";
                return Page();
            }

            if (await _db.Vendors.AnyAsync(v => v.AadhaarNumber == aadhaar))
            {
                ErrorMessage = "This Aadhaar number is already registered.";
                return Page();
            }

            if (!NoPan && await _db.Vendors.AnyAsync(v => v.PanNumber == pan))
            {
                ErrorMessage = "This PAN number is already registered.";
                return Page();
            }

            string aadhaarFrontPath;
            string aadhaarBackPath;
            string? panPath = null;
            try
            {
                aadhaarFrontPath = await KycStorage.SaveAsync(AadhaarFrontPhoto!, _env.ContentRootPath, "aadhaar-front");
                aadhaarBackPath = await KycStorage.SaveAsync(AadhaarBackPhoto!, _env.ContentRootPath, "aadhaar-back");
                if (!NoPan)
                {
                    panPath = await KycStorage.SaveAsync(PanPhoto!, _env.ContentRootPath, "pan");
                }
            }
            catch (InvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }

            var user = new AppUser
            {
                PhoneNumber = digits,
                PhoneDisplay = Phone.Trim(),
                FullName = FullName.Trim(),
                Email = email,
                PasswordHash = PasswordHelper.Hash(Password),
                Role = UserRoles.Vendor,
                City = City,
                Pincode = pincode,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var vendor = new Models.Vendor
            {
                UserId = user.Id,
                BusinessName = FullName.Trim(),
                Phone = Phone.Trim(),
                City = City,
                Pincode = pincode,
                Services = Services,
                IsAvailable = false,
                AadhaarNumber = aadhaar,
                AadhaarPhotoPath = aadhaarFrontPath,
                AadhaarBackPhotoPath = aadhaarBackPath,
                PanNumber = NoPan ? null : pan,
                PanPhotoPath = panPath,
                VerificationStatus = VendorVerification.Pending,
                CreatedAt = DateTime.UtcNow
            };
            _db.Vendors.Add(vendor);
            await _db.SaveChangesAsync();

            StaffSession.SignIn(Request, Response, StaffSession.VendorApp, user, vendor.Id);
            return RedirectToPage("/Index");
        }
    }
}
