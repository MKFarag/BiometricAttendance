using System.Security.Cryptography;

namespace BiometricAttendance.Domain.Entities;

public sealed class InstructorPass
{
    public int Id { get; set; }
    public string PassCode { get; private set; }
    public DateTime GeneratedAt { get; private set; }
    public DateTime ExpiredAt { get; private set; }
    public bool IsUsed { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiredAt;
    public bool IsValid => !IsExpired && !IsUsed;

    public InstructorPass()
    {
        GeneratedAt = DateTime.UtcNow;
        ExpiredAt = GeneratedAt.AddDays(1);
        PassCode = GeneratePassCode();
    }

    private static string GeneratePassCode() =>
        Convert.ToHexString(RandomNumberGenerator.GetBytes(8));
}