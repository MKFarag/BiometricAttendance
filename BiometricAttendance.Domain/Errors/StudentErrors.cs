namespace BiometricAttendance.Domain.Errors;

public record StudentErrors
{
    public static readonly Error SetDataFailed =
        new("Student.SetDataFailed", "Failed to set the student data.", StatusCodes.BadRequest);
}
