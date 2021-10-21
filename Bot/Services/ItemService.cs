using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RatingsBot.Data;
using RatingsBot.Models;

namespace RatingsBot.Services
{
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

        public async Task<IReadOnlyCollection<Item>> ListAsync(string query)
        {
            return await _context.Items
                .Where(i => EF.Functions.ILike(i.Name, $"%{query}%"))
                .ToListAsync();
        }
    }
}