using System.Runtime.Serialization;

namespace BiometricAttendance.Domain.Entities;

public sealed class Student
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int Level { get; private set; }
    public int DepartmentId { get; private set; }
    public int? FingerprintId { get; private set; }

    public Department Department { get; private set; } = default!;
    public Fingerprint? Fingerprint { get; private set; }
    public List<Attendance> Attendances { get; private set; } = [];
    public List<StudentCourse> Courses { get; private set; } = [];

    [IgnoreDataMember]
    public string? Name { get; private set; }

    public string? DepartmentName => Department?.Name;
    public bool CanPromote => Level < 5;

    public static Student Create(string userId, int level, int departmentId)
    {
        if (level > 5 || level <= 0)
            throw new ArgumentOutOfRangeException(nameof(level));

        return new Student
        {
            UserId = userId,
            Level = level,
            DepartmentId = departmentId
        };
    }

    public void SetName(string name)
        => Name = name;

    public void ChangeLevel(int level)
    {
        if (level > 5 || level <= 0)
            throw new ArgumentOutOfRangeException(nameof(level));

        ResetData();

        Level = level;
    }

    public void ChangeDepartment(int newDepartmentId)
    {
        if (newDepartmentId == DepartmentId)
            throw new InvalidOperationException("Student is already in the specified department.");

        ResetData();

        DepartmentId = newDepartmentId;
    }

    public void Promote()
    {
        if (Level >= 5)
            throw new InvalidOperationException("Student is already at the maximum level.");

        ResetData();

        Level++;
    }

    public void ResetData()
    {
        Attendances.Clear();
        Courses.Clear();
    }

    public void SetFingerprint(Fingerprint fingerprint)
    {
        if (FingerprintId.HasValue)
            throw new InvalidOperationException("Student already has a fingerprint assigned.");

        Fingerprint = fingerprint;
        FingerprintId = fingerprint.Id;
    }

    public void RemoveFingerprint()
    {
        FingerprintId = null;
        Fingerprint = null;
    }

    public void AssignFingerprint(int fingerprintId)
    {
        FingerprintId = fingerprintId;
    }

    public void EnrollInCourses(IEnumerable<int> coursesId)
    {
        foreach (var courseId in coursesId)
        {
            if (Courses.Any(sc => sc.CourseId == courseId))
                throw new InvalidOperationException("Student is already enrolled in the specified course.");

            Courses.Add(StudentCourse.Create(Id, courseId));
        }
    }
}
