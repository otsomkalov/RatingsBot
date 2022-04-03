using System.Collections.Immutable;
using Bot.Constants;
using Telegram.Bot.Types;

namespace Bot.Models;

public class RatingCallbackQueryData : CallbackQueryData
{
    public RatingCallbackQueryData(CallbackQuery query) : base(query)
    {
        var callbackData = query.Data.Split(ReplyMarkup.Separator).ToImmutableList();

        ItemId = int.Parse(callbackData[0]);
        RatingValue = int.TryParse(callbackData[2], out var id) ? id : 0;
    }

    public int RatingValue { get; }
}