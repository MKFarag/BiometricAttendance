namespace BiometricAttendance.Application.Contracts.Courses;

public record CourseRequest(
    string Name,
    string Code,
    int Level
);

#region Validation

public class CourseRequestValidator : AbstractValidator<CourseRequest>
{
    public CourseRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .Matches(RegexPatterns.OnlyLettersWithSpaces)
            .WithMessage("You must enter only letters and spaces")
            .MaximumLength(100);

        RuleFor(x => x.Code)
            .NotEmpty()
            .Matches(RegexPatterns.AlphanumericUnderscore)
            .WithMessage("You must enter only letters, numbers, and underscores")
            .MaximumLength(25);

        RuleFor(x => x.Level)
            .GreaterThan(0);
    }
}

#endregion
