namespace BiometricAttendance.Domain.Errors;

public record DepartmentErrors
{
    public static readonly Error NotFound =
        new("Department.NotFound", "No department found", StatusCodes.NotFound);

    public static readonly Error NameAlreadyExists =
        new("Department.NameAlreadyExists", "A department with the same name already exists", StatusCodes.Conflict);

    public static readonly Error InUse =
        new("Department.InUse", "This department is used right now", StatusCodes.Conflict);
}
