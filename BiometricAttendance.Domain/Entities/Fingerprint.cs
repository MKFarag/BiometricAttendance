namespace BiometricAttendance.Domain.Entities;

public sealed class Fingerprint
{
    public int Id { get; private set; }
    public int StudentId { get; private set; }
    public DateTime RegisteredAt { get; private set; }

    public Student Student { get; private set; } = default!;

    public static Fingerprint Create(int fingerprintId, int studentId)
        => new() { Id = fingerprintId, StudentId = studentId, RegisteredAt = DateTime.UtcNow };
}
