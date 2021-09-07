using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using RatingsBot.Constants;
using RatingsBot.Data;
using RatingsBot.Helpers;
using RatingsBot.Models;
using RatingsBot.Resources;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineQueryResults;

namespace RatingsBot.Services
{
    public class InlineQueryService
    {
        private readonly AppDbContext _context;
        private readonly ITelegramBotClient _bot;
        private readonly IStringLocalizer<Messages> _localizer;

        public InlineQueryService(AppDbContext context, ITelegramBotClient bot, IStringLocalizer<Messages> localizer)
        {
            _context = context;
            _bot = bot;
            _localizer = localizer;
        }

        public async Task HandleAsync(InlineQuery inlineQuery)
        {
            var items = await _context.Items
                .Where(i => EF.Functions.ILike(i.Name, $"%{inlineQuery.Query}%"))
                .ToListAsync();

            var itemsArticles = items.Select(item =>
            {
                var currentUserRating = item.Ratings.FirstOrDefault(r => r.UserId == inlineQuery.From.Id);
                var avgRating = item.Ratings.Any()
                    ? item.Ratings.Sum(r => r.Value) / item.Ratings.Count
                    : 0;

                var currentRatingString = currentUserRating == null
                    ? "No rating"
                    : string.Join(string.Empty, Enumerable.Repeat("⭐", currentUserRating.Value));

                var avgRatingString = avgRating == 0
                    ? "No average rating"
                    : string.Join(string.Empty, Enumerable.Repeat("⭐", avgRating));

                var content = string.Format(_localizer[ResourcesNames.ItemMessageTemplate], item.Name, item.Category?.Name,
                    item.Place?.Name, currentRatingString, avgRatingString);

                return new InlineQueryResultArticle(item.Id.ToString(), item.Name, new InputTextMessageContent(content))
                {
                    Description = string.Format(_localizer[ResourcesNames.ItemInlineArticleTemplate], item.Category?.Name, item.Place?.Name,
                        currentRatingString),
                    ReplyMarkup = ReplyMarkupHelpers.GetRatingsMarkup(item.Id)
                };
            });

            await _bot.AnswerInlineQueryAsync(inlineQuery.Id, itemsArticles, 1);
        }
    }
}