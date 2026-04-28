namespace BiometricAttendance.Application.Contracts.Instructors;

public record InstructorPassResponse(
    int Id,
    string PassCode,
    int Remaining
);
