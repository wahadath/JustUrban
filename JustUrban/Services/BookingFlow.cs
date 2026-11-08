namespace JustUrban.Services
{
    public static class BookingFlow
    {
        public static string DisplayServiceName(string? serviceKey)
        {
            return serviceKey?.Trim().ToLowerInvariant() switch
            {
                "touchwood" => "Touch Wood Polishing",
                "melamine" => "Melamine Polishing",
                "texture" => "Texture Painting",
                "moulding" => "Moulding",
                _ => string.IsNullOrWhiteSpace(serviceKey) ? "Home Makeover" : serviceKey
            };
        }

        public static void KeepBookingTempData(Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataDictionary tempData)
        {
            foreach (var key in new[]
            {
                "ServiceName", "UserPickedDate", "UserPickedTime",
                "CustomerName", "ContactNumber", "City", "Pincode", "StreetAddress", "AmountDue"
            })
            {
                tempData.Keep(key);
            }
        }
    }
}
