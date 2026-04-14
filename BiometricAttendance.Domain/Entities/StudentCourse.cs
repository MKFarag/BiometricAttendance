namespace BiometricAttendance.Domain.Entities;

public sealed class StudentCourse
{
    public int StudentId { get; set; }
    public int CourseId { get; set; }

    public Student Student { get; set; } = default!;
    public Course Course { get; set; } = default!;
}
