using Microsoft.EntityFrameworkCore;
using RatingsBot.Models;

namespace RatingsBot.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }

        public DbSet<Item> Items { get; set; }

        public DbSet<Rating> Ratings { get; set; }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("uuid-ossp");

            modelBuilder.Entity<BaseEntity>(builder =>
            {
                builder.HasKey(e => e.Id);

                builder.Property(e => e.Id)
                    .HasDefaultValueSql("uuid_generate_v4()");
            });
        }
    }
}
