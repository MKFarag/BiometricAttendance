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
}
