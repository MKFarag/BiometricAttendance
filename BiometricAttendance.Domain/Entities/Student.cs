namespace BiometricAttendance.Domain.Entities;

public sealed class Student
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int Level { get; set; }
    public int DepartmentId { get; set; }
    public int? FingerprintId { get; set; }

    public Department Department { get; set; } = default!;
    public Fingerprint? Fingerprint { get; set; }
    public List<Attendance> Attendances { get; set; } = [];
    public List<StudentCourse> Courses { get; set; } = [];

    public string DepartmentName => Department.Name;
    public bool CanPromote => Level < 5;

    public static Student Create(string userId, int level, int departmentId, int? fingerprintId = null)
    {
        if (level > 5 || level <= 0)
            throw new ArgumentOutOfRangeException(nameof(level));

        return new Student
        {
            UserId = userId,
            Level = level,
            DepartmentId = departmentId,
            FingerprintId = fingerprintId
        };
    }

    public void ChangeDepartment(int newDepartmentId)
    {
        if (newDepartmentId == DepartmentId)
            throw new InvalidOperationException("Student is already in the specified department.");

        DepartmentId = newDepartmentId;
    }

    public void Promote()
    {
        if (Level >= 5)
            throw new InvalidOperationException("Student is already at the maximum level.");

        Level++;
    }
}
