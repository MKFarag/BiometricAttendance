namespace BiometricAttendance.Infrastructure.Persistence.Configurations;

internal sealed class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId);

        builder
            .HasOne(s => s.Fingerprint)
            .WithOne(f => f.Student)
            .HasForeignKey<Fingerprint>(f => f.StudentId);
    }
}
