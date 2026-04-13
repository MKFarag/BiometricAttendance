namespace BiometricAttendance.Domain.Entities;

public sealed class Fingerprint
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Student Student { get; set; } = default!;
}
