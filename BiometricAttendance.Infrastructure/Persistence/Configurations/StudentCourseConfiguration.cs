namespace BiometricAttendance.Infrastructure.Persistence.Configurations;

public class StudentCourseConfiguration : IEntityTypeConfiguration<StudentCourse>
{
    public void Configure(EntityTypeBuilder<StudentCourse> builder)
    {
        builder
            .HasKey(x => new { x.StudentId, x.CourseId });
    }
}
