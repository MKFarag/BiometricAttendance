namespace BiometricAttendance.Application.Contracts.Profile;

public record ChangeUserNameRequest(
    string NewUserName
);

#region Validation

public class ChangeUserNameRequestValidator : AbstractValidator<ChangeUserNameRequest>
{
    public ChangeUserNameRequestValidator()
    {
        RuleFor(x => x.NewUserName)
            .NotEmpty()
            .Matches(RegexPatterns.AlphanumericUnderscore)
            .Length(3, 50);
    }
}

#endregion
