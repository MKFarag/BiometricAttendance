namespace BiometricAttendance.Domain.Errors;

public record AttendanceErrors
{
    public static readonly Error AlreadyMarked =
        new("Attendance.AlreadyMarked", "Attendance for this student in this course and week is already marked.", StatusCodes.Conflict);

    public static readonly Error StudentNotEnrolled =
        new("Attendance.StudentNotEnrolled", "The student is not enrolled in the specified course.", StatusCodes.BadRequest);

    public static readonly Error WeekAlreadyRecorded = 
        new("Attendance.WeekAlreadyRecorded", "Attendance for this week has already been recorded for the specified course.", StatusCodes.Conflict);
}
