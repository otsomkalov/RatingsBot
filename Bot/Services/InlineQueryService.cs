using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RatingsBot.Data;
using RatingsBot.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RatingsBot.Services
{
    public class InlineQueryService
    {
        private readonly AppDbContext _context;
        private readonly ITelegramBotClient _bot;

        public InlineQueryService(AppDbContext context, ITelegramBotClient bot)
        {
            _context = context;
            _bot = bot;
        }

        public async Task HandleAsync(InlineQuery inlineQuery)
        {
            var items = await _context.Ratings
                .AsNoTracking()
                .Include(i => i.Item)
                .Where(r => EF.Functions.Like(r.Item.Name, $"%{inlineQuery.Query}%"))
                .Where(r => r.UsedId == inlineQuery.From.Id)
                .ToListAsync();

            var itemsArticles = items.Select(InlineQueryHelpers.GetArticle);

            await _bot.AnswerInlineQueryAsync(inlineQuery.Id, itemsArticles);
        }
    }
}