namespace BiometricAttendance.Application.Contracts.Departments;

public record DepartmentRequest(
    string Name
);

#region Validation

public class DepartmentRequestValidator : AbstractValidator<DepartmentRequest>
{
    public DepartmentRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .Matches(RegexPatterns.OnlyLettersWithSpaces)
            .WithMessage("You must enter only letters and spaces")
            .MaximumLength(100);
    }
}

#endregion