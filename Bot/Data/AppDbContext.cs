using System.Reflection;
using Bot.Models;
using Microsoft.EntityFrameworkCore;

namespace Bot.Data;

public class AppDbContext : DbContext
{
    public DbSet<Category> Categories { get; set; }

    public DbSet<Item> Items { get; set; }

    public DbSet<Rating> Ratings { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Place> Places { get; set; }

    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}