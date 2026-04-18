namespace BiometricAttendance.Application.Contracts.Students;

public record ChangeStudentDepartmentRequest(
    int DepartmentId
);

#region Validation

public class ChangeStudentDepartmentRequestValidator : AbstractValidator<ChangeStudentDepartmentRequest>
{
    public ChangeStudentDepartmentRequestValidator()
    {
        RuleFor(x => x.DepartmentId)
            .GreaterThan(0);
    }
}

#endregion