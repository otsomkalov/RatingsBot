using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RatingsBot.Models;

namespace RatingsBot.Data.Configuration
{
    public class ItemConfiguration : BaseEntityConfiguration<Item>
    {
        public override void Configure(EntityTypeBuilder<Item> builder)
        {
            base.Configure(builder);

            builder.HasMany(i => i.Ratings)
                .WithOne(r => r.Item)
                .HasForeignKey(r => r.ItemId);

            builder.HasOne(i => i.Category)
                .WithMany(c => c.Items)
                .HasForeignKey(i => i.CategoryId);
        }
    }
}