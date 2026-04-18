namespace BiometricAttendance.Domain.Entities;

public sealed class Attendance
{
    public int Id { get; set; }
    public int StudentId { get; private set; }
    public int CourseId { get; private set; }
    public int WeekNumber { get; private set; }
    public DateTime MarkedAt { get; private set; } = DateTime.UtcNow;

    public Student Student { get; private set; } = default!;
    public Course Course { get; private set; } = default!;
}
