namespace BiometricAttendance.Infrastructure.Persistence.Configurations;

internal sealed class InstructorPassConfiguration : IEntityTypeConfiguration<InstructorPass>
{
    public void Configure(EntityTypeBuilder<InstructorPass> builder)
    {
        builder
            .Property(x => x.PassCode)
            .HasMaxLength(25);
    }
}
