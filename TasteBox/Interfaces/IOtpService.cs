using TasteBox.Abstractions;

namespace TasteBox.Interfaces;

public interface IOtpService
{
    Task<Result> SendOtpAsync(string email);
    Task<Result<ApplicationUser>> VerifyOtpAsync(string email, string code);
}