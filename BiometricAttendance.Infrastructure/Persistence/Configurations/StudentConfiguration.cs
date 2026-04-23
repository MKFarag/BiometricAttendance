namespace BiometricAttendance.Infrastructure.Persistence.Configurations;

internal sealed class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder
            .HasOne<ApplicationUser>()
            .WithOne()
            .HasForeignKey<Student>(x => x.UserId);

        builder
            .HasOne(s => s.Fingerprint)
            .WithOne(f => f.Student)
            .HasForeignKey<Domain.Entities.Fingerprint>(f => f.StudentId);
    }
}
