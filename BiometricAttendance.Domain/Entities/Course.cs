namespace BiometricAttendance.Domain.Entities;

public sealed class Course
{
    public int Id { get; set; }
    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public int DepartmentId { get; private set; }

    public Department Department { get; private set; } = default!;

    public static Course Create(string name, string code, int departmentId)
    {
        return new Course
        {
            Name = name,
            Code = code,
            DepartmentId = departmentId
        };
    }

    public void Update(string name, string code, int departmentId)
    {
        Name = name;
        Code = code;
        DepartmentId = departmentId;
    }
}
