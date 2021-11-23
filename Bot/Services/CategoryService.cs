using Bot.Data;
using Bot.Models;
using Microsoft.EntityFrameworkCore;

namespace Bot.Services;

public class CategoryService
{
    private readonly AppDbContext _context;

    public CategoryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<Category>> ListAsync()
    {
        return await _context.Categories.AsNoTracking()
            .ToListAsync();
    }

    public async Task AddAsync(string name)
    {
        await _context.AddAsync(new Category
        {
            Name = name
        });

        await _context.SaveChangesAsync();
    }
}