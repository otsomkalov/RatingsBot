using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RatingsBot.Models;

namespace RatingsBot.Data.Configuration
{
    public class RatingConfiguration : BaseEntityConfiguration<Rating>
    {
        public override void Configure(EntityTypeBuilder<Rating> builder)
        {
            base.Configure(builder);

            builder.HasOne(r => r.Item)
                .WithMany(i => i.Ratings)
                .HasForeignKey(r => r.ItemId);

            builder.HasOne(r => r.User)
                .WithMany(u => u.Ratings)
                .HasForeignKey(r => r.UserId);
        }
    }
}