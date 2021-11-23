using Bot.Helpers;
using Bot.Resources;
using Microsoft.Extensions.Localization;
using Telegram.Bot.Types;

namespace Bot.Services;

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
        var items = await _itemService.ListAsync(inlineQuery.Query, Constants.Telegram.MaximumInlineResults);

        var itemsArticles = items
            .Select(item => InlineQueryResultHelpers.GetItemQueryResult(item, inlineQuery, _localizer[Messages.ItemMessageTemplate]));

        await _bot.AnswerInlineQueryAsync(inlineQuery.Id, itemsArticles);
    }
}