namespace BiometricAttendance.Domain.Entities;

public sealed class Attendance
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public int WeekNumber { get; set; }
    public DateTime MarkedAt { get; set; } = DateTime.UtcNow;

    public Student Student { get; set; } = default!;
    public Course Course { get; set; } = default!;
}
