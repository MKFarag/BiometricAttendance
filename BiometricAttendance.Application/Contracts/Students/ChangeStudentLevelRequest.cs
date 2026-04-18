namespace BiometricAttendance.Application.Contracts.Students;

public record ChangeStudentLevelRequest(
    int Level
);

#region Validation

public class ChangeStudentLevelRequestValidator : AbstractValidator<ChangeStudentLevelRequest>
{
    public ChangeStudentLevelRequestValidator()
    {
        RuleFor(x => x.Level)
            .InclusiveBetween(1, 5);
    }
}

#endregion