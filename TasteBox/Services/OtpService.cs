using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity.UI.Services;
using TasteBox.Abstractions;

namespace TasteBox.Services;

public class OtpService(
    UserManager<ApplicationUser> userManager,
    ApplicationDbContext context,
    IEmailSender emailSender) : IOtpService
{
    public async Task<Result> SendOtpAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return Result.Failure(UserErrors.InvalidCredentials);

        var otp = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

        context.OtpCodes.Add(new OtpCode
        {
            UserId = user.Id,
            Code = otp,
            ExpireAt = DateTime.UtcNow.AddMinutes(5),
            IsUsed = false
        });

        await context.SaveChangesAsync();

        await emailSender.SendEmailAsync(user.Email!, "OTP Code", $"Your OTP is: {otp}");

        return Result.Success();
    }

    public async Task<Result<ApplicationUser>> VerifyOtpAsync(string email, string code)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return Result.Failure<ApplicationUser>(UserErrors.InvalidCredentials);

        var otp = await context.OtpCodes
            .Where(x => x.UserId == user.Id && !x.IsUsed && x.ExpireAt > DateTime.UtcNow)
            .OrderByDescending(x => x.Id)
            .FirstOrDefaultAsync();

        if (otp is null || otp.Code != code)
            return Result.Failure<ApplicationUser>(UserErrors.InvalidCode);

        otp.IsUsed = true;
        await context.SaveChangesAsync();

        return Result.Success(user);
    }
}