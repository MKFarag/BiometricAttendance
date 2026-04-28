namespace BiometricAttendance.Domain.Errors;

public static class FingerprintErrors
{
    public static readonly Error AlreadyWorking =
        new("Fingerprint.AlreadyWorking", "The fingerprint reader is already running.", StatusCodes.Conflict);

    public static readonly Error EnrollmentAlreadyWorking =
        new("Fingerprint.EnrollmentAlreadyWorking", "The enrollment is working right now.", StatusCodes.BadRequest);

    public static readonly Error AttendanceActionAlreadyWorking =
        new("Fingerprint.AttendanceActionAlreadyWorking", "The attendance action is working right now.", StatusCodes.BadRequest);

    public static readonly Error StartFailed =
        new("Fingerprint.StartFailed", "Failed to start the fingerprint reader. Check the serial port connection.", StatusCodes.InternalServerError);

    public static readonly Error ServiceUnavailable =
        new("Fingerprint.ServiceUnavailable", "The fingerprint reader service is not running.", StatusCodes.ServiceUnavailable);

    public static readonly Error NoResponse =
        new("Fingerprint.NoResponse", "No response received from the fingerprint reader within the expected time.", StatusCodes.RequestTimeout);

    public static readonly Error NoData =
        new("Fingerprint.NoData", "No fingerprint data is available.", StatusCodes.NotFound);

    public static readonly Error InvalidData =
        new("Fingerprint.InvalidData", "The received fingerprint data is not in a valid format.", StatusCodes.BadRequest);

    public static readonly Error InvalidPassword =
        new("Fingerprint.InvalidPassword", "The provided password is incorrect.", StatusCodes.Forbidden);
}
