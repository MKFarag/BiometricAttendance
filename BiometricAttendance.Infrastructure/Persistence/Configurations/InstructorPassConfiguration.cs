using Microsoft.EntityFrameworkCore.ChangeTracking;
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
        //TODO
            //.Metadata.SetValueComparer(new ValueComparer<List<string>>(
            //    (c1, c2) => c1!.SequenceEqual(c2!),
            //    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            //    c => c.ToList())
            //);
    }
}
