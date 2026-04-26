namespace BiometricAttendance.Application.Contracts.Attendances;

public record EndTakingAttendanceRequest(
    int CourseId,
    int WeekNum
);

#region Validation

public class EndTakingAttendanceRequestValidator : AbstractValidator<EndTakingAttendanceRequest>
{
    public EndTakingAttendanceRequestValidator()
    {
        RuleFor(x => x.CourseId)
            .GreaterThan(0);

        RuleFor(x => x.WeekNum)
            .GreaterThan(0);
    }
}

#endregion
