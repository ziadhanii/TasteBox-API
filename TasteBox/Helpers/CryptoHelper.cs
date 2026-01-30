using System.Security.Cryptography;

namespace TasteBox.Helpers;

public static class CryptoHelper
{
    private const int OtpMinValue = 100000;
    private const int OtpMaxValue = 999999;
    private const int TokenByteLength = 32;
    private const int RefreshTokenByteLength = 64;


    public static string GenerateOtp()
        => RandomNumberGenerator.GetInt32(OtpMinValue, OtpMaxValue + 1).ToString();


    public static string GenerateResetToken()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(TokenByteLength));


    public static string GenerateRefreshToken()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(RefreshTokenByteLength));


    public static string ComputeHash(string input)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(input)));


    public static bool VerifyHash(string input, string expectedHash)
    {
        var inputHash = ComputeHash(input);
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(inputHash),
            Encoding.UTF8.GetBytes(expectedHash));
    }
}