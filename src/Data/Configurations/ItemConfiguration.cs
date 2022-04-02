using Core.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configuration;

public class ItemConfiguration : BaseEntityConfiguration<Item>
{
    public override void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.HasMany(i => i.Ratings)
            .WithOne(r => r.Item)
            .HasForeignKey(r => r.ItemId);

        builder.HasOne(i => i.Category)
            .WithMany(c => c.Items)
            .HasForeignKey(i => i.CategoryId);

        base.Configure(builder);
    }
}