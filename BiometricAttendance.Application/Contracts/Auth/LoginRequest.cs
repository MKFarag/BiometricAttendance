namespace BiometricAttendance.Application.Contracts.Auth;

public record LoginRequest(
    string Identifier,
    string Password
);

#region Validation

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Identifier)
            .NotEmpty();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}

#endregion
