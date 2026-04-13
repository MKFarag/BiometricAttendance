namespace BiometricAttendance.Domain.Entities;

public sealed class DepartmentCourse
{
    public int DepartmentId { get; set; }
    public int CourseId { get; set; }

    public Department Department { get; set; } = default!;
    public Course Course { get; set; } = default!;
}
