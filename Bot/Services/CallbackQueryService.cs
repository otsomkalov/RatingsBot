using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using RatingsBot.Constants;
using RatingsBot.Helpers;
using RatingsBot.Models;
using RatingsBot.Resources;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace RatingsBot.Services
{
    public class CallbackQueryService
    {
        private readonly ITelegramBotClient _bot;
        private readonly IStringLocalizer<Messages> _localizer;
        private readonly CategoryService _categoryService;
        private readonly PlaceService _placeService;
        private readonly ItemService _itemService;
        private readonly RatingService _ratingService;

        public CallbackQueryService(ITelegramBotClient bot, IStringLocalizer<Messages> localizer, CategoryService categoryService,
            PlaceService placeService, ItemService itemService, RatingService ratingService)
        {
            _bot = bot;
            _localizer = localizer;
            _categoryService = categoryService;
            _placeService = placeService;
            _itemService = itemService;
            _ratingService = ratingService;
        }

        public async Task HandleAsync(CallbackQuery callbackQuery)
        {
            var callbackData = callbackQuery.Data.Split(ReplyMarkup.Separator).ToImmutableList();

            var itemId = int.Parse(callbackData[0]);
            int? entityId = int.TryParse(callbackData[2], out var id) ? id : null;
            var item = await _itemService.GetAsync(itemId);

            await (callbackData[1] switch
            {
                ReplyMarkup.Category => ProcessCategoryCommand(callbackQuery, entityId, item),
                ReplyMarkup.Place => ProcessPlaceCommand(callbackQuery, entityId, item),
                ReplyMarkup.Rating => ProcessRatingCommand(callbackQuery, entityId, item)
            });
        }

        private async Task ProcessCategoryCommand(CallbackQuery callbackQuery, int? entityId, Item item)
        {
            if (!entityId.HasValue)
            {
                var categories = await _categoryService.ListAsync();

                await _bot.EditMessageReplyMarkupAsync(new(callbackQuery.From.Id),
                    callbackQuery.Message.MessageId,
                    ReplyMarkupHelpers.GetCategoriesMarkup(item.Id, categories));
            }
            else
            {
                await _itemService.UpdateCategoryAsync(item, entityId.Value);

                var places = await _placeService.ListAsync();

                await _bot.EditMessageTextAsync(new(callbackQuery.From.Id),
                    callbackQuery.Message.MessageId,
                    _localizer[ResourcesNames.Place],
                    replyMarkup: ReplyMarkupHelpers.GetPlacesMarkup(item.Id, places));
            }
        }

        private async Task ProcessPlaceCommand(CallbackQuery callbackQuery, int? entityId, Item item)
        {
            if (entityId is -1)
            {
                var places = await _placeService.ListAsync();

                await _bot.EditMessageReplyMarkupAsync(new(callbackQuery.From.Id),
                    callbackQuery.Message.MessageId,
                    ReplyMarkupHelpers.GetPlacesMarkup(item.Id, places));
            }
            else
            {
                await _itemService.UpdatePlaceAsync(item, entityId);

                await _bot.EditMessageTextAsync(new(callbackQuery.From.Id),
                    callbackQuery.Message.MessageId,
                    MessageHelpers.GetItemMessageText(item, callbackQuery.From.Id, _localizer[ResourcesNames.ItemMessageTemplate]),
                    replyMarkup: ReplyMarkupHelpers.GetRatingsMarkup(item.Id));
            }
        }

        private async Task ProcessRatingCommand(CallbackQuery callbackQuery, int? entityId, Item item)
        {
            if (entityId.HasValue)
            {
                await _ratingService.UpsertAsync(callbackQuery.From.Id, item.Id, entityId.Value);

                await _bot.AnswerCallbackQueryAsync(callbackQuery.Id, _localizer[ResourcesNames.Recorded]);
            }

            var messageText = MessageHelpers.GetItemMessageText(item, callbackQuery.From.Id, _localizer[ResourcesNames.ItemMessageTemplate]);

            try
            {
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
            catch (MessageIsNotModifiedException)
            {
                await _bot.AnswerCallbackQueryAsync(callbackQuery.Id,
                    _localizer[ResourcesNames.Refreshed]);
            }
        }
    }
}