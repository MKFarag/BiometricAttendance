namespace BiometricAttendance.Domain.Entities;

public sealed class Course
{
    public int Id { get; set; }
    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public int Level { get; private set; }

    public List<DepartmentCourse> DepartmentCourses { get; private set; } = [];

    public static Course Create(string name, string code, int level)
    {
        if (level > 5 || level <= 0)
            throw new ArgumentOutOfRangeException(nameof(level));

        return new Course
        {
            Name = name,
            Code = code,
            Level = level
        };
    }
}
