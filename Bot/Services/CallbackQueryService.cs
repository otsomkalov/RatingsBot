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
using Telegram.Bot.Types.Enums;

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

            switch (callbackData[1])
            {
                case ReplyMarkup.Category:

                    item.CategoryId = entityId;

                    _context.Update(item);
                    await _context.SaveChangesAsync();

                    var places = await _context.Places.AsNoTracking()
                        .ToListAsync();

                    await _bot.EditMessageTextAsync(new(callbackQuery.From.Id),
                        callbackQuery.Message.MessageId,
                        _localizer[ResourcesNames.Place],
                        replyMarkup: ReplyMarkupHelpers.GetPlacesMarkup(callbackData[0], places));

                    break;

                case ReplyMarkup.Place:

                    item.PlaceId = entityId;

                    _context.Update(item);
                    await _context.SaveChangesAsync();

                    await _bot.AnswerCallbackQueryAsync(callbackQuery.Id,
                        _localizer[ResourcesNames.Created]);

                    await _bot.DeleteMessageAsync(new(callbackQuery.From.Id), callbackQuery.Message.MessageId);

                    var users = await _context.Users.AsNoTracking()
                        .ToListAsync();

                    foreach (var user in users)
                    {
                        await _bot.SendTextMessageAsync(new(user.Id),
                            string.Format(_localizer[ResourcesNames.ItemTemplate], item.Name, item.Category?.Name, item.Place?.Name),
                            ParseMode.Markdown,
                            replyMarkup: ReplyMarkupHelpers.GetRatingsMarkup(itemId));

                        await Task.Delay(300);
                    }

                    break;

                case ReplyMarkup.Rating:

                    if (!await _context.Ratings.AnyAsync(r => r.ItemId == itemId && r.UserId == callbackQuery.From.Id))
                    {
                        var rating = new Rating
                        {
                            ItemId = itemId,
                            UserId = callbackQuery.From.Id,
                            Value = entityId
                        };

                        await _context.AddAsync(rating);
                        await _context.SaveChangesAsync();
                    }

                    await _bot.AnswerCallbackQueryAsync(callbackQuery.Id, _localizer[ResourcesNames.Recorded]);

                    break;
            }
        }
    }
}
