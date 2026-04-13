namespace BiometricAttendance.Infrastructure.Persistence.Repositories;

public class UserRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : IUserRepository
{
    private readonly ApplicationDbContext _context = context;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    #region Read Operations

    public async Task<User?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
        => await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == id)
            .ProjectToType<User>()
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await _context.Users
            .AsNoTracking()
            .Where(u => u.NormalizedEmail == _userManager.NormalizeEmail(email))
            .ProjectToType<User>()
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<IEnumerable<TProjection>> GetAllProjectionWithRolesAsync<TProjection>(bool includeDefaultRole, CancellationToken cancellationToken = default)
        where TProjection : class
        => await (from u in _context.Users
                  join ur in _context.UserRoles
                  on u.Id equals ur.UserId
                  join r in _context.Roles
                  on ur.RoleId equals r.Id into roles
                  where includeDefaultRole || !roles.Any(x => x.IsDefault)
                  select new
                  {
                      u.Id,
                      u.FirstName,
                      u.LastName,
                      u.Email,
                      u.UserName,
                      u.IsDisabled,
                      Roles = roles.Select(x => x.Name).ToList()
                  })
                  .AsNoTracking()
                  .GroupBy(u => new { u.Id, u.FirstName, u.LastName, u.Email, u.UserName, u.IsDisabled })
                  .Select(u => new
                  {
                      u.Key.Id,
                      FullName = $"{u.Key.FirstName}, {u.Key.LastName}",
                      u.Key.Email,
                      u.Key.UserName,
                      u.Key.IsDisabled,
                      Roles = u.SelectMany(x => x.Roles).Distinct()
                  })
                  .ProjectToType<TProjection>()
                  .ToListAsync(cancellationToken);

    public async Task<(IEnumerable<string> roles, IEnumerable<string> permissions)> GetRolesAndPermissionsAsync(User user, CancellationToken cancellationToken = default)
    {
        var roles = await _userManager.GetRolesAsync(user.Adapt<ApplicationUser>());

        var permissions = await (from r in _context.Roles
                                 join p in _context.RoleClaims
                                 on r.Id equals p.RoleId
                                 where roles.Contains(r.Name!) && p.ClaimType == Permissions.Type
                                 select p.ClaimValue!)
                                 .AsNoTracking()
                                 .Distinct()
                                 .ToListAsync(cancellationToken);

        return (roles, permissions);
    }

    public async Task<IList<string>> GetRolesAsync(User user)
        => await _userManager.GetRolesAsync(user.Adapt<ApplicationUser>());

    #endregion

    #region Check Operations

    public async Task<bool> IsLockedOutAsync(User user)
    {
        var applicationUser = await GetByIdOrThrowAsync(user.Id);

        return await _userManager.IsLockedOutAsync(applicationUser);
    }

    public async Task<bool> IsEmailConfirmedAsync(User user)
    {
        var applicationUser = await GetByIdOrThrowAsync(user.Id);

        return await _userManager.IsEmailConfirmedAsync(applicationUser);
    }

    public async Task<bool> IsChangeUserNameAvailable(User user)
    {
        var applicationUser = await GetByIdOrThrowAsync(user.Id);

        return applicationUser.UserNameChangeAvailableAt == null || applicationUser.UserNameChangeAvailableAt <= DateTime.UtcNow;
    }

    public async Task<bool> ExistsAsync(string userId, CancellationToken cancellationToken = default)
        => await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Id == userId, cancellationToken);

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        email = _userManager.NormalizeEmail(email);

        return await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.NormalizedEmail == email, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, string userId, CancellationToken cancellationToken = default)
    {
        email = _userManager.NormalizeEmail(email);

        return await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.NormalizedEmail == email && u.Id != userId, cancellationToken);
    }

    public async Task<bool> UserNameExistsAsync(string userName, CancellationToken cancellationToken = default)
    {
        userName = _userManager.NormalizeName(userName);

        return await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.NormalizedUserName == userName, cancellationToken);
    }

    public async Task<bool> UserNameExistsAsync(string userName, string userId, CancellationToken cancellationToken = default)
    {
        userName = _userManager.NormalizeName(userName);

        return await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.NormalizedUserName == userName && u.Id != userId, cancellationToken);
    }

    #endregion

    #region Token & Code Operations

    public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
    {
        var applicationUser = await GetByIdOrThrowAsync(user.Id);

        return await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);
    }

    public async Task<Result> ConfirmEmailWithTokenAsync(User user, string token)
    {
        var applicationUser = await GetByIdOrThrowAsync(user.Id);

        var result = await _userManager.ConfirmEmailAsync(applicationUser, token);

        return result.ToDomain();
    }

    public async Task<string> GenerateChangeEmailTokenAsync(User user, string newEmail)
    {
        var applicationUser = await GetByIdOrThrowAsync(user.Id);

        return await _userManager.GenerateChangeEmailTokenAsync(applicationUser, newEmail);
    }

    public async Task<Result> ChangeEmailAsync(User user, string newEmail, string token)
    {
        var applicationUser = await GetByIdOrThrowAsync(user.Id);

        var result = await _userManager.ChangeEmailAsync(applicationUser, newEmail, token);

        return result.ToDomain();
    }

    public async Task<string> GeneratePasswordResetTokenAsync(User user)
    {
        var applicationUser = await GetByIdOrThrowAsync(user.Id);

        return await _userManager.GeneratePasswordResetTokenAsync(applicationUser);
    }

    public async Task<Result> ResetPasswordAsync(User user, string token, string newPassword)
    {
        var applicationUser = await GetByIdOrThrowAsync(user.Id);

        var result = await _userManager.ResetPasswordAsync(applicationUser, token, newPassword);

        return result.ToDomain();
    }

    #endregion

    #region Modify Operations

    public async Task<Result> CreateAsync(User user, string password, bool confirmEmail = false)
    {
        var applicationUser = user.CreateIdentity();

        if (confirmEmail)
            applicationUser.EmailConfirmed = true;

        var result = await _userManager.CreateAsync(applicationUser, password);

        if (result.Succeeded)
            user.Id = applicationUser.Id;

        return result.ToDomain();
    }

    public async Task<Result> UpdateAsync(User user)
    {
        var applicationUser = await GetByIdOrThrowAsync(user.Id);

        applicationUser = user.Adapt(applicationUser);

        var result = await _userManager.UpdateAsync(applicationUser);

        return result.ToDomain();
    }

    public async Task<Result> SetLockoutEndDateAsync(User user, DateTimeOffset? lockoutEnd)
    {
        var applicationUser = await GetByIdOrThrowAsync(user.Id);

        var result = await _userManager.SetLockoutEndDateAsync(applicationUser, lockoutEnd);

        return result.ToDomain();
    }

    public async Task<Result> ChangePasswordAsync(User user, string currentPassword, string newPassword)
    {
        var applicationUser = await GetByIdOrThrowAsync(user.Id);

        var result = await _userManager.ChangePasswordAsync(applicationUser, currentPassword, newPassword);

        return result.ToDomain();
    }

    public async Task<Result> ChangeUserNameAsync(User user, string newUserName, int minDaysBetweenChanges)
    {
        var applicationUser = await GetByIdOrThrowAsync(user.Id);

        var result = await _userManager.SetUserNameAsync(applicationUser, newUserName);

        if (result.Succeeded)
        {
            applicationUser.UserNameChangeAvailableAt = DateTime.UtcNow.AddDays(minDaysBetweenChanges);
            result = await _userManager.UpdateAsync(applicationUser);
        }

        return result.ToDomain();
    }

    public async Task<Result> AddToRolesAsync(User user, IEnumerable<string> roles)
    {
        var applicationUser = await GetByIdOrThrowAsync(user.Id);

        var result = await _userManager.AddToRolesAsync(applicationUser, roles);

        return result.ToDomain();
    }

    public async Task<Result> AddToRoleAsync(User user, string role)
    {
        var applicationUser = await GetByIdOrThrowAsync(user.Id);

        var result = await _userManager.AddToRoleAsync(applicationUser, role);

        return result.ToDomain();
    }

    public async Task DeleteAllRolesAsync(User user)
        => await _context.UserRoles
            .Where(ur => ur.UserId == user.Id)
            .ExecuteDeleteAsync();

    public async Task AddRefreshTokenAsync(User user, RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        var applicationUser = await GetByIdOrThrowAsync(user.Id);

        applicationUser.RefreshTokens.Add(refreshToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> RevokeRefreshTokenAsync(User user, string token, CancellationToken cancellationToken = default)
    {
        var applicationUser = await GetByIdOrThrowAsync(user.Id);

        var userRefreshToken = applicationUser.RefreshTokens.FirstOrDefault(rt => rt.Token == token && rt.IsActive);

        if (userRefreshToken is null)
            return Result.Failure(UserErrors.InvalidRefreshToken);

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task RemoveExpiredRefreshTokensAsync(CancellationToken cancellationToken = default)
        => await _context.Set<RefreshToken>()
            .Where(t => t.ExpiresOn < DateTime.UtcNow || t.RevokedOn != null)
            .ExecuteDeleteAsync(cancellationToken);

    public async Task ToggleStatusAsync(User user, CancellationToken cancellationToken = default)
        => await _context.Users
            .Where(u => u.Id == user.Id)
            .ExecuteUpdateAsync
            (
                u => u.SetProperty(x => x.IsDisabled, x => !x.IsDisabled)
                , cancellationToken
            );

    #endregion

    #region Private Methods

    private async Task<ApplicationUser> GetByIdOrThrowAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user is not null
            ? user
            : throw new InvalidOperationException(string.Format("User with ID '{0}' not found. This indicates a bug in the service layer validation.", userId));
    }

    #endregion
}
