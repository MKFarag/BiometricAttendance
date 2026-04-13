namespace BiometricAttendance.Presentation.DTOs.Common;

public class FileNameValidator : AbstractValidator<IFormFile>
{
    public FileNameValidator()
    {
        RuleFor(x => x.FileName)
            .Matches(RegexPatterns.SafeFileName)
            .WithMessage("Invalid file name")
            .When(x => x is not null);
    }
}
