namespace BiometricAttendance.Domain.Errors;

public record StudentErrors
{
    public static readonly Error NotFound =
        new("Student.NotFound", "The student was not found.", StatusCodes.NotFound);

    public static readonly Error SetDataFailed =
        new("Student.SetDataFailed", "Failed to set the student data.", StatusCodes.BadRequest);
}
