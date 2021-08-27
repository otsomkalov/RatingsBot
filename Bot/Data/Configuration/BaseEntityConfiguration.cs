using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RatingsBot.Models;

namespace RatingsBot.Data.Configuration
{
    public class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T: BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()");
        }
    }
}