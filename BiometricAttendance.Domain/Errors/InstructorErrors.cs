namespace BiometricAttendance.Domain.Errors;

public record InstructorErrors
{
    public static readonly Error InvalidPassword =
        new("Instructor.InvalidPassword", "The provided password is invalid.", StatusCodes.BadGateway);

    public static readonly Error NoPassAvailable =
        new("Instructor.NoPassAvailable", "There is no available pass.", StatusCodes.NotFound);

    public static readonly Error SetRoleFailed =
        new("Instructor.SetRoleFailed", "Failed to set instructor role.", StatusCodes.BadRequest);
}
