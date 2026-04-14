namespace BiometricAttendance.Infrastructure.Persistence.Configurations;

internal class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
{
    public void Configure(EntityTypeBuilder<Attendance> builder)
    {
        builder
            .HasIndex(x => new { x.StudentId, x.CourseId })
            .IsUnique();
    }
}
