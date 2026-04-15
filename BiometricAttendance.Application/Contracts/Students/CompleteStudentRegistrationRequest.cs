namespace BiometricAttendance.Application.Contracts.Students;

public record CompleteStudentRegistrationRequest(
    int Level,
    int DepartmentId
);

#region Validation

public class CompleteStudentRegistrationRequestValidator : AbstractValidator<CompleteStudentRegistrationRequest>
{
    public CompleteStudentRegistrationRequestValidator()
    {
        RuleFor(x => x.Level)
            .InclusiveBetween(1, 5);

        RuleFor(x => x.DepartmentId)
            .GreaterThan(0);
    }
}

#endregion