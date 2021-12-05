using Bot.Models;
using Microsoft.EntityFrameworkCore;

namespace Bot.Services;

public class ItemService
{
    private readonly AppDbContext _context;

    public ItemService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> AddAsync(string name)
    {
        var newItem = new Item
        {
            Name = name
        };

        await _context.AddAsync(newItem);
        await _context.SaveChangesAsync();

        return newItem.Id;
    }

    public async Task UpdateCategoryAsync(Item item, int categoryId)
    {
        item.CategoryId = categoryId;

        _context.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePlaceAsync(Item item, int? placeId)
    {
        item.PlaceId = placeId;

        _context.Update(item);
        await _context.SaveChangesAsync();
    }

    public ValueTask<Item> GetAsync(int itemId)
    {
        return _context.Items.FindAsync(itemId);
    }

    public async Task<IReadOnlyCollection<Item>> ListAsync(string query, int count)
    {
        return await _context.Items
            .Include(i => i.Category)
            .Include(i => i.Place)
            .Include(i => i.Ratings)
            .Where(i =>
                EF.Functions.ILike(i.Name, $"%{query}%") ||
                EF.Functions.ILike(i.Category.Name, $"%{query}%") ||
                EF.Functions.ILike(i.Place.Name, $"%{query}%"))
            .Take(count)
            .ToListAsync();
    }
}