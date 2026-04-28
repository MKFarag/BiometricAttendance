namespace BiometricAttendance.Domain.Entities;

public sealed class Attendance
{
    public int Id { get; set; }
    public int StudentId { get; private set; }
    public int CourseId { get; private set; }
    public int WeekNumber { get; private set; }
    public DateTime MarkedAt { get; private set; }

    public Student Student { get; private set; } = default!;
    public Course Course { get; private set; } = default!;

    public static Attendance Create(int studentId, int courseId, int weekNumber)
        => new() { StudentId = studentId, CourseId = courseId, WeekNumber = weekNumber, MarkedAt = DateTime.UtcNow };

    public static List<Attendance> CreateRange(List<int> studentsId, int courseId, int weekNumber)
        => [.. studentsId.Select(studentId => Create(studentId, courseId, weekNumber))];
}
