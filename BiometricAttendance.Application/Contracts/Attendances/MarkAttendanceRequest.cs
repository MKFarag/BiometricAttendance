namespace BiometricAttendance.Application.Contracts.Attendances;

public record MarkAttendanceRequest(
    int StudentId,
    int CourseId,
    int WeekNumber
);

#region Validation

public class MarkAttendanceRequestValidator : AbstractValidator<MarkAttendanceRequest>
{
    public MarkAttendanceRequestValidator()
    {
        RuleFor(x => x.StudentId)
            .GreaterThan(0);

        RuleFor(x => x.CourseId)
            .GreaterThan(0);

        RuleFor(x => x.WeekNumber)
            .GreaterThan(0);
    }
}

#endregion
