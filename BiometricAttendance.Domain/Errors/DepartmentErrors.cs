namespace BiometricAttendance.Domain.Errors;

public record DepartmentErrors
{
    public static readonly Error NotFound =
        new("Department.NotFound", "No department found", StatusCodes.NotFound);
}
