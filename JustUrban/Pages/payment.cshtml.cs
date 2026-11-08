using JustUrban.Data;
using JustUrban.Models;
using JustUrban.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace JustUrban.Pages
{
    public class paymentModel : PageModel
    {
        private readonly AppDbContext _db;

        public paymentModel(AppDbContext db)
        {
            _db = db;
        }

        public void OnGet()
        {
            BookingFlow.KeepBookingTempData(TempData);
        }

        public async Task<IActionResult> OnPostAsync(string paymentMethod)
        {
            const decimal amount = 49.00m;

            try
            {
                if (paymentMethod != "Online" && paymentMethod != "Offline")
                {
                    TempData["PopupType"] = "Failure";
                    TempData["PopupMessage"] = "Please choose a payment method.";
                    BookingFlow.KeepBookingTempData(TempData);
                    return RedirectToPage();
                }

                if (paymentMethod == "Online")
                {
                    bool isGatewayTransactionSuccessful = true;
                    if (!isGatewayTransactionSuccessful)
                    {
                        TempData["PopupType"] = "Failure";
                        TempData["PopupMessage"] = "The bank rejected your online transaction. Please check your balance and try again.";
                        BookingFlow.KeepBookingTempData(TempData);
                        return RedirectToPage();
                    }
                }

                DateTime.TryParse(TempData["UserPickedDate"]?.ToString(), out var slotDate);
                if (slotDate == default)
                {
                    slotDate = DateTime.Today;
                }

                // 1. Retrieve logged-in UserId
                int? userId = AuthSession.UserId(Request);

                // 2. Fallback to TempData if AuthSession isn't set
                if (userId == null && TempData["UserId"] != null)
                {
                    if (int.TryParse(TempData["UserId"]?.ToString(), out int parsedId))
                    {
                        userId = parsedId;
                    }
                }

                // 3. Secondary Fallback: Match by ContactNumber if UserId is still null
                string contactNumber = TempData["ContactNumber"]?.ToString() ?? string.Empty;
                if (userId == null && !string.IsNullOrEmpty(contactNumber))
                {
                    string normalized = AuthSession.NormalizePhone(contactNumber);
                    var matchedUser = await _db.Users.FirstOrDefaultAsync(u => u.PhoneNumber == normalized);
                    if (matchedUser != null)
                    {
                        userId = matchedUser.Id;
                    }
                }

                var booking = new Booking
                {
                    UserId = userId, // Assigning Foreign Key to prevent NULL
                    ServiceName = TempData["ServiceName"]?.ToString() ?? "Home Makeover",
                    CustomerName = TempData["CustomerName"]?.ToString() ?? "",
                    ContactNumber = contactNumber,
                    City = TempData["City"]?.ToString(),
                    Pincode = TempData["Pincode"]?.ToString(),
                    StreetAddress = TempData["StreetAddress"]?.ToString(),
                    SlotDate = slotDate.Date,
                    SlotTime = TempData["UserPickedTime"]?.ToString() ?? "",
                    PaymentMethod = paymentMethod,
                    Amount = amount,
                    Status = "Confirmed",
                    CreatedAt = DateTime.UtcNow
                };

                _db.Bookings.Add(booking);
                await _db.SaveChangesAsync();

                return RedirectToPage("/Tracking", new { id = booking.Id });
            }
            catch (Exception ex)
            {
                TempData["PopupType"] = "Failure";
                TempData["PopupMessage"] = $"Could not save your booking: {ex.Message}";
                BookingFlow.KeepBookingTempData(TempData);
                return RedirectToPage();
            }
        }
    }
}