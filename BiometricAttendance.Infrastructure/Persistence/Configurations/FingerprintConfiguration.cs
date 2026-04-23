namespace BiometricAttendance.Infrastructure.Persistence.Configurations;

internal sealed class FingerprintConfiguration : IEntityTypeConfiguration<Domain.Entities.Fingerprint>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Fingerprint> builder)
    {
        builder
            .HasIndex(x => x.StudentId)
            .IsUnique();

        builder
            .Property(f => f.Id)
            .ValueGeneratedNever();
    }
}
