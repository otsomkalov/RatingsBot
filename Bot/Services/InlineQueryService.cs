using Microsoft.Extensions.Localization;
using RatingsBot.Constants;
using RatingsBot.Resources;
using Telegram.Bot.Types;

namespace RatingsBot.Services;

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

        var itemsArticles = items.Select(item => InlineQueryResultHelpers.GetItemQueryResult(item, inlineQuery, _localizer[ResourcesNames.ItemMessageTemplate])).Take(50);

        await _bot.AnswerInlineQueryAsync(inlineQuery.Id, itemsArticles);
    }
}