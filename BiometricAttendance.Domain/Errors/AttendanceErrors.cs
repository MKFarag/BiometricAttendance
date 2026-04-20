namespace BiometricAttendance.Domain.Errors;

public record AttendanceErrors
{
    public static readonly Error AlreadyMarked =
        new("Attendance.AlreadyMarked", "Attendance for this student in this course and week is already marked.", StatusCodes.Conflict);

    public static readonly Error StudentNotEnrolled =
        new("Attendance.StudentNotEnrolled", "The student is not enrolled in the specified course.", StatusCodes.BadRequest);
}
