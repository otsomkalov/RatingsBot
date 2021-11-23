using Bot.Models;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineQueryResults;

namespace Bot.Helpers;

public static class InlineQueryResultHelpers
{
    public static InlineQueryResultArticle GetItemQueryResult(Item item, InlineQuery inlineQuery, string messageTemplate)
    {
        var description = MessageHelpers.GetItemMessageText(item, inlineQuery.From.Id, messageTemplate);

        return new(item.Id.ToString(), item.Name, new InputTextMessageContent(description))
        {
            Description = description,
            ReplyMarkup = ReplyMarkupHelpers.GetRatingsMarkup(item.Id)
        };
    }
}