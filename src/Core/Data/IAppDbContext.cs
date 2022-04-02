using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Data;

public interface IAppDbContext
{
    DbSet<Category> Categories { get; set; }

    DbSet<Item> Items { get; set; }

    DbSet<Manufacturer> Manufacturers { get; set; }

    DbSet<Place> Places { get; set; }

    DbSet<Rating> Ratings { get; set; }

    DbSet<User> Users { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}