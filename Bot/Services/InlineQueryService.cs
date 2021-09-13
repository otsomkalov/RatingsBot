using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using RatingsBot.Constants;
using RatingsBot.Helpers;
using RatingsBot.Resources;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineQueryResults;

namespace RatingsBot.Services
{
    public class InlineQueryService
    {
        private readonly ITelegramBotClient _bot;
        private readonly IStringLocalizer<Messages> _localizer;
        private readonly ItemService _itemService;

        public InlineQueryService(ITelegramBotClient bot, IStringLocalizer<Messages> localizer, ItemService itemService)
        {
            _bot = bot;
            _localizer = localizer;
            _itemService = itemService;
        }

        public async Task HandleAsync(InlineQuery inlineQuery)
        {
            var items = await _itemService.ListAsync(inlineQuery.Query);

            var itemsArticles = items.Select(item =>
            {
                var description =
                    MessageHelpers.GetItemMessageText(item, inlineQuery.From.Id, _localizer[ResourcesNames.ItemMessageTemplate]);

                return new InlineQueryResultArticle(item.Id.ToString(), item.Name, new InputTextMessageContent(description))
                {
                    Description = description,
                    ReplyMarkup = ReplyMarkupHelpers.GetRatingsMarkup(item.Id)
                };
            });

            await _bot.AnswerInlineQueryAsync(inlineQuery.Id, itemsArticles);
        }
    }
}