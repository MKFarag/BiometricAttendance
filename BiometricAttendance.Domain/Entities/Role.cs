namespace BiometricAttendance.Domain.Entities;

public sealed class Role
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsDisabled { get; set; }
    public bool IsDefault { get; set; }
}
