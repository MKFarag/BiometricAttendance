namespace BiometricAttendance.Presentation.DTOs.Common;

public class ImageExtensionValidator : AbstractValidator<IFormFile>
{
    public ImageExtensionValidator()
    {
        RuleFor(x => x)
            .Must((request, context) =>
            {
                var extension = Path.GetExtension(request.FileName.ToLower());
                return FileSettings.AllowedImagesExtensions.Contains(extension);
            })
            .WithMessage("Image extension is not allowed")
            .When(x => x is not null);
    }
}