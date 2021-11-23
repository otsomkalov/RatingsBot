using Bot.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bot.Data.Configuration;

public class PlaceConfiguration : BaseEntityConfiguration<Place>
{
    public override void Configure(EntityTypeBuilder<Place> builder)
    {
        builder.HasMany(p => p.Items)
            .WithOne(i => i.Place)
            .HasForeignKey(i => i.PlaceId);

        base.Configure(builder);
    }
}