namespace BiometricAttendance.Application.Contracts.Auth;

public record GetInstructorRoleRequest(string Pass);

#region Validation

public class GetInstructorRoleRequestValidator : AbstractValidator<GetInstructorRoleRequest>
{
    public GetInstructorRoleRequestValidator()
    {
        RuleFor(x => x.Pass)
            .NotEmpty();
    }
}

#endregion
