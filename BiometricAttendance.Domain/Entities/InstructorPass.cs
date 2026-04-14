using System.Security.Cryptography;

namespace BiometricAttendance.Domain.Entities;

public sealed class InstructorPass(int maxUses)
{
    public int Id { get; set; }
    public string PassCode { get; private set; } = GeneratePassCode();
    public int MaxUsedCount { get; private set; } = maxUses;
    public DateTime GeneratedAt { get; private set; } = DateTime.UtcNow;
    public bool IsDisabled { get; private set; } = false;
    public List<string> UsedBy { get; private set; } = [];

    public bool IsExhausted => UsedBy.Count >= MaxUsedCount;

    public InstructorPass() : this(10) { }

    public void Use(string userId)
    {
        if (UsedBy.Contains(userId))
            throw new InvalidOperationException("This pass has already been used.");

        if (IsExhausted || IsDisabled)
            throw new InvalidOperationException("This pass has expired.");

        UsedBy!.Add(userId);
    }

    public void Renew()
    {
        if (IsDisabled)
            throw new InvalidOperationException("This pass is disabled.");

        if (UsedBy.Count != 0)
            throw new InvalidOperationException("This pass is used before.");

        PassCode = GeneratePassCode();
        GeneratedAt = DateTime.UtcNow;
    }

    public void Disable() => IsDisabled = true;

    private static string GeneratePassCode() =>
        Convert.ToHexString(RandomNumberGenerator.GetBytes(8));
}