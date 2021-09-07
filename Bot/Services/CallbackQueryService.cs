using System.Collections.Immutable;
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

namespace RatingsBot.Services
{
    public class CallbackQueryService
    {
        private readonly AppDbContext _context;
        private readonly ITelegramBotClient _bot;
        private readonly IStringLocalizer<Messages> _localizer;

        public CallbackQueryService(AppDbContext context, ITelegramBotClient bot, IStringLocalizer<Messages> localizer)
        {
            _context = context;
            _bot = bot;
            _localizer = localizer;
        }

        public async Task HandleAsync(CallbackQuery callbackQuery)
        {
            var callbackData = callbackQuery.Data.Split(ReplyMarkup.Separator).ToImmutableList();

            var itemId = int.Parse(callbackData[0]);
            var entityId = int.Parse(callbackData[2]);

            var item = await _context.Items.FindAsync(itemId);

            await (callbackData[1] switch
            {
                ReplyMarkup.Category => ProcessCategoryCommand(callbackQuery, entityId, item),
                ReplyMarkup.Place => ProcessPlaceCommand(callbackQuery, entityId, item),
                ReplyMarkup.Rating => ProcessRatingCommand(callbackQuery, item, entityId)
            });
        }

        private async Task ProcessCategoryCommand(CallbackQuery callbackQuery, int entityId, Item item)
        {
            if (entityId == 0)
            {
                var categories = await _context.Categories.AsNoTracking().ToListAsync();

                await _bot.EditMessageReplyMarkupAsync(new(callbackQuery.From.Id),
                    callbackQuery.Message.MessageId,
                    ReplyMarkupHelpers.GetCategoriesMarkup(item.Id, categories));
            }
            else
            {
                item.CategoryId = entityId;

                _context.Update(item);
                await _context.SaveChangesAsync();

                var places = await _context.Places.AsNoTracking()
                    .ToListAsync();

                await _bot.EditMessageTextAsync(new(callbackQuery.From.Id),
                    callbackQuery.Message.MessageId,
                    _localizer[ResourcesNames.Place],
                    replyMarkup: ReplyMarkupHelpers.GetPlacesMarkup(item.Id, places));
            }
        }

        private async Task ProcessPlaceCommand(CallbackQuery callbackQuery, int entityId, Item item)
        {
            if (entityId == 0)
            {
                var places = await _context.Places.AsNoTracking().ToListAsync();

                await _bot.EditMessageReplyMarkupAsync(new(callbackQuery.From.Id),
                    callbackQuery.Message.MessageId,
                    ReplyMarkupHelpers.GetPlacesMarkup(item.Id, places));
            }
            else
            {
                item.PlaceId = entityId;

                _context.Update(item);
                await _context.SaveChangesAsync();

                var currentUserRating = item.Ratings.FirstOrDefault(r => r.UserId == callbackQuery.From.Id);
                var avgRating = item.Ratings.Any()
                    ? item.Ratings.Sum(r => r.Value) / item.Ratings.Count
                    : 0;

                var currentRatingString = currentUserRating == null
                    ? "No rating"
                    : string.Join(string.Empty, Enumerable.Repeat("⭐", currentUserRating.Value));

                var avgRatingString = avgRating == 0
                    ? "No average rating"
                    : string.Join(string.Empty, Enumerable.Repeat("⭐", avgRating));

                var messageText = string.Format(_localizer[ResourcesNames.ItemMessageTemplate], item.Name, item.Category?.Name, item.Place?.Name,
                    currentRatingString, avgRatingString);

                await _bot.EditMessageTextAsync(new(callbackQuery.From.Id),
                    callbackQuery.Message.MessageId,
                    messageText,
                    replyMarkup: ReplyMarkupHelpers.GetRatingsMarkup(item.Id));
            }
        }

        private async Task ProcessRatingCommand(CallbackQuery callbackQuery, Item item, int entityId)
        {
            var rating = await _context.Ratings.FirstOrDefaultAsync(r => r.UserId == callbackQuery.From.Id && r.ItemId == item.Id);

            if (rating != null)
            {
                rating.Value = entityId;

                _context.Update(rating);
            }
            else
            {
                rating = new()
                {
                    ItemId = item.Id,
                    UserId = callbackQuery.From.Id,
                    Value = entityId
                };

                await _context.AddAsync(rating);
            }

            await _context.SaveChangesAsync();

            await _bot.AnswerCallbackQueryAsync(callbackQuery.Id, _localizer[ResourcesNames.Recorded]);

            var currentUserRating = item.Ratings.FirstOrDefault(r => r.UserId == callbackQuery.From.Id);
            var avgRating = item.Ratings.Any()
                ? item.Ratings.Sum(r => r.Value) / item.Ratings.Count
                : 0;

            var currentRatingString = currentUserRating == null
                ? "No rating"
                : string.Join(string.Empty, Enumerable.Repeat("⭐", currentUserRating.Value));

            var avgRatingString = avgRating == 0
                ? "No average rating"
                : string.Join(string.Empty, Enumerable.Repeat("⭐", avgRating));

            var messageText = string.Format(_localizer[ResourcesNames.ItemMessageTemplate], item.Name, item.Category?.Name, item.Place?.Name,
                currentRatingString, avgRatingString);

            if (callbackQuery.InlineMessageId != null)
            {
                await _bot.EditMessageTextAsync(callbackQuery.InlineMessageId,
                    messageText,
                    replyMarkup: ReplyMarkupHelpers.GetRatingsMarkup(item.Id));
            }
            else
            {
                await _bot.EditMessageTextAsync(new(callbackQuery.From.Id),
                    callbackQuery.Message.MessageId,
                    messageText,
                    replyMarkup: ReplyMarkupHelpers.GetRatingsMarkup(item.Id));
            }
        }
    }
}