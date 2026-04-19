namespace BiometricAttendance.Domain.Entities;

public sealed class Department
{
    public int Id { get; set; }
    public string Name { get; private set; } = string.Empty;

    public static Department Create(string name)
        => new() { Name = name };

    public void Update(string name)
        => Name = name;
}
