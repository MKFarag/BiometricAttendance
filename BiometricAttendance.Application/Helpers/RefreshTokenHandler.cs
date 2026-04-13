using System.Security.Cryptography;

namespace BiometricAttendance.Application.Helpers;

public static class RefreshTokenHandler
{
    private const int _refreshTokenExpiryDays = 14;
    private const int _autoRemoveAfterDays = 7;

    internal static string GenerateNewToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    internal static int ExpiryDays => _refreshTokenExpiryDays;
    public static int AutoRemoveAfterDays => _autoRemoveAfterDays;
}
