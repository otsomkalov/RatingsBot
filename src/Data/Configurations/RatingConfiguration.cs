using Core.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configuration;

public class RatingConfiguration : BaseEntityConfiguration<Rating>
{
    public override void Configure(EntityTypeBuilder<Rating> builder)
    {
        builder.HasOne(r => r.Item)
            .WithMany(i => i.Ratings)
            .HasForeignKey(r => r.ItemId);

        builder.HasOne(r => r.User)
            .WithMany(u => u.Ratings)
            .HasForeignKey(r => r.UserId);

        base.Configure(builder);
    }
}