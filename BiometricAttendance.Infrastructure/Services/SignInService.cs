namespace BiometricAttendance.Infrastructure.Services;

public class SignInService(UserManager<ApplicationUser> userManager) : ISignInService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<Result<User>> PasswordSignInAsync(string identifier, string password, bool lockoutOnFailure)
    {
        var user = identifier.Contains('@')
            ? await _userManager.FindByEmailAsync(identifier)
            : await _userManager.FindByNameAsync(identifier);

        if (user is null)
            return Result.Failure<User>(UserErrors.InvalidCredentials);

        if (lockoutOnFailure && await _userManager.IsLockedOutAsync(user))
            return Result.Failure<User>(UserErrors.LockedUser);

        if (!user.EmailConfirmed)
            return Result.Failure<User>(UserErrors.EmailNotConfirmed);

        if (!await _userManager.CheckPasswordAsync(user, password))
        {
            if (lockoutOnFailure)
                await _userManager.AccessFailedAsync(user);

            return Result.Failure<User>(UserErrors.InvalidCredentials);
        }

        if (lockoutOnFailure && user.AccessFailedCount > 0)
            await _userManager.ResetAccessFailedCountAsync(user);

        return Result.Success(user.Adapt<User>());
    }
}