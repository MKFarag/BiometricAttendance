namespace BiometricAttendance.Domain.Entities;

public sealed class User
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsDisabled { get; set; }

    public string FullName => $"{FirstName} {LastName}";
}
