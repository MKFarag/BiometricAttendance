namespace BiometricAttendance.Infrastructure.Persistence.Configurations;

internal sealed class DepartmentCourseConfiguration : IEntityTypeConfiguration<DepartmentCourse>
{
    public void Configure(EntityTypeBuilder<DepartmentCourse> builder)
    {
        builder.HasKey(x => new { x.DepartmentId, x.CourseId });
    }
}
