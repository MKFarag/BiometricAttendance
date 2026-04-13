namespace BiometricAttendance.Infrastructure.Persistence.Configurations;

internal sealed class FingerprintConfiguration : IEntityTypeConfiguration<Fingerprint>
{
    public void Configure(EntityTypeBuilder<Fingerprint> builder)
    {
        builder
            .HasIndex(x => x.StudentId)
            .IsUnique();

        builder
            .Property(f => f.Id)
            .ValueGeneratedNever();
    }
}
