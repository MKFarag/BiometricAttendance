namespace BiometricAttendance.Infrastructure.Persistence.Configurations;

internal class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        // Default data

        builder.HasData(new ApplicationRole
        {
            Id = DefaultRoles.Admin.Id,
            Name = DefaultRoles.Admin.Name,
            NormalizedName = DefaultRoles.Admin.Name.ToUpper(),
            ConcurrencyStamp = DefaultRoles.Admin.ConcurrencyStamp,
            IsDefault = false,
        },
        new ApplicationRole
        {
            Id = DefaultRoles.Student.Id,
            Name = DefaultRoles.Student.Name,
            NormalizedName = DefaultRoles.Student.Name.ToUpper(),
            ConcurrencyStamp = DefaultRoles.Student.ConcurrencyStamp,
            IsDefault = true,
        },
        new ApplicationRole
        {
            Id = DefaultRoles.Instructor.Id,
            Name = DefaultRoles.Instructor.Name,
            NormalizedName = DefaultRoles.Instructor.Name.ToUpper(),
            ConcurrencyStamp = DefaultRoles.Instructor.ConcurrencyStamp,
            IsDefault = false,
        });
    }
}
