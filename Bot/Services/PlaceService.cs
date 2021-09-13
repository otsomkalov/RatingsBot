using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RatingsBot.Data;
using RatingsBot.Models;

namespace RatingsBot.Services
{
    public class PlaceService
    {
        private readonly AppDbContext _context;

        public PlaceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyCollection<Place>> ListAsync()
        {
            return await _context.Places.AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(string name)
        {
            await _context.AddAsync(new Place
            {
                Name = name
            });

            await _context.SaveChangesAsync();
        }
    }
}