namespace JustUrban.Auth
{
    // Vendor Aadhaar authentication.
    // UIDAI sends the OTP to the mobile number already registered on the Aadhaar card.
    // Leave this commented until the licensed KYC provider is connected.
    public static class AadhaarOtpClient
    {
        // public static async Task<string> SendOtpAsync(string aadhaarNumber, CancellationToken cancellationToken = default)
        // {
        //     // 1. Post the 12-digit Aadhaar number to the KYC provider.
        //     // 2. The provider asks UIDAI to text an OTP to the Aadhaar-linked mobile.
        //     // 3. Return the provider transaction id for VerifyOtpAsync.
        //     //
        //     // using var response = await http.PostAsJsonAsync(
        //     //     "/aadhaar/otp",
        //     //     new { aadhaarNumber },
        //     //     cancellationToken);
        //     // response.EnsureSuccessStatusCode();
        //     // var body = await response.Content.ReadFromJsonAsync<AadhaarOtpResult>(cancellationToken);
        //     // return body!.TransactionId;
        //     throw new NotImplementedException();
        // }

        // public static async Task<bool> VerifyOtpAsync(string transactionId, string otp, CancellationToken cancellationToken = default)
        // {
        //     // Post the OTP the vendor received on the Aadhaar-registered mobile.
        //     // Return true only when UIDAI accepts that OTP.
        //     //
        //     // using var response = await http.PostAsJsonAsync(
        //     //     "/aadhaar/otp/verify",
        //     //     new { transactionId, otp },
        //     //     cancellationToken);
        //     // return response.IsSuccessStatusCode;
        //     throw new NotImplementedException();
        // }

        // private sealed class AadhaarOtpResult
        // {
        //     public string TransactionId { get; set; } = string.Empty;
        // }
    }
}
