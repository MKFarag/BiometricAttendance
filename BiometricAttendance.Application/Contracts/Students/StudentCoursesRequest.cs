namespace BiometricAttendance.Application.Contracts.Students;

public record StudentCoursesRequest(
    IEnumerable<int> CoursesId
);

#region Validation

public class StudentCoursesRequestValidator : AbstractValidator<StudentCoursesRequest>
{
    public StudentCoursesRequestValidator()
    {
        RuleFor(x => x.CoursesId)
            .NotEmpty()
            .Must(x => x.Count() == x.Distinct().Count())
            .WithMessage("Course IDs must be unique.")
            .When(x => x.CoursesId != null);
    }
}

#endregion