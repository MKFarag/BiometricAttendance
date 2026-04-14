using System.Text.Json;

namespace BiometricAttendance.Infrastructure.Persistence.Configurations;

internal sealed class InstructorPassConfiguration : IEntityTypeConfiguration<InstructorPass>
{
    public void Configure(EntityTypeBuilder<InstructorPass> builder)
    {
        builder
            .Property(x => x.PassCode)
            .HasMaxLength(25);

        builder
            .Property(x => x.UsedBy)
            .HasConversion
            (
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<List<string>>(v, JsonSerializerOptions.Default)!
            );
    }
}
