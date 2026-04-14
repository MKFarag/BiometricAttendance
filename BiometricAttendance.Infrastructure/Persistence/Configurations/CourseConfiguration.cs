namespace BiometricAttendance.Infrastructure.Persistence.Configurations;

internal sealed class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder
            .Property(x => x.Name)
            .HasMaxLength(100);

        builder
            .Property(x => x.Code)
            .HasMaxLength(25);

        builder
            .HasIndex(x => x.Name)
            .IsUnique();

        builder
            .HasIndex(x => x.Code)
            .IsUnique();
    }
}
