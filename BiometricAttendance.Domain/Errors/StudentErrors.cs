namespace BiometricAttendance.Domain.Errors;

public record StudentErrors
{
    public static readonly Error NotFound =
        new("Student.NotFound", "The student was not found.", StatusCodes.NotFound);

    public static readonly Error InvalidCourses =
        new("Student.InvalidCourses", "One or more selected courses are not allowed for this student.", StatusCodes.BadRequest);

    public static readonly Error SetDataFailed =
        new("Student.SetDataFailed", "Failed to set the student data.", StatusCodes.BadRequest);

    public static readonly Error PromoteFailed =
        new("Student.PromoteFailed", "Failed to promote the student.", StatusCodes.BadRequest);
}
