using System.Collections.Immutable;
using Bot.Constants;
using Telegram.Bot.Types;

namespace Bot.Models;

public class EntitiesCallbackQueryData : CallbackQueryData
{
    public EntitiesCallbackQueryData(CallbackQuery query) : base(query)
    {
        var callbackData = query.Data.Split(ReplyMarkup.Separator).ToImmutableList();

        ItemId = int.Parse(callbackData[0]);

        if (callbackData.Count == 4)
        {
            Page = int.Parse(callbackData[2]);
            EntityId = int.TryParse(callbackData[3], out var id) ? id : null;
        }
        else
        {
            Page = 0;
            EntityId = 0;
        }
    }

    public int Page { get; }
}