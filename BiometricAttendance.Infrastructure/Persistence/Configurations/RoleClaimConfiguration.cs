namespace BiometricAttendance.Infrastructure.Persistence.Configurations;

internal class RoleClaimConfiguration : IEntityTypeConfiguration<IdentityRoleClaim<string>>
{
    public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> builder)
    {
        var id = 1;

        var adminClaims = Permissions.GetAll()
            .Select(permission => new IdentityRoleClaim<string>
            {
                Id = id++,
                RoleId = DefaultRoles.Admin.Id,
                ClaimType = Permissions.Type,
                ClaimValue = permission
            });

        var instructorClaims = Permissions.GetAllForInstructor()
            .Select(permission => new IdentityRoleClaim<string>
            {
                Id = id++,
                RoleId = DefaultRoles.Instructor.Id,
                ClaimType = Permissions.Type,
                ClaimValue = permission
            });

        var superInstructorClaims = Permissions.GetAllForSuperInstructor()
            .Select(permission => new IdentityRoleClaim<string>
            {
                Id = id++,
                RoleId = DefaultRoles.SuperInstructor.Id,
                ClaimType = Permissions.Type,
                ClaimValue = permission
            });

        var studentClaims = Permissions.GetAllForStudent()
            .Select(permission => new IdentityRoleClaim<string>
            {
                Id = id++,
                RoleId = DefaultRoles.Student.Id,
                ClaimType = Permissions.Type,
                ClaimValue = permission
            });

        builder.HasData([
            .. adminClaims,
            .. instructorClaims,
            .. superInstructorClaims,
            .. studentClaims
        ]);
    }
}
