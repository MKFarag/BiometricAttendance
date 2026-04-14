namespace BiometricAttendance.Domain.Errors;

public record CourseErrors
{
    public static readonly Error NotFound =
        new("Course.NotFound", "No course found", StatusCodes.NotFound);

    public static readonly Error NameAlreadyExists =
        new("Course.NameAlreadyExists", "A course with the same name already exists", StatusCodes.Conflict);

    public static readonly Error CodeAlreadyExists =
        new("Course.CodeAlreadyExists", "A course with the same code already exists", StatusCodes.Conflict);

    public static readonly Error InUse =
        new("Course.InUse", "This course is used right now", StatusCodes.Conflict);
}
