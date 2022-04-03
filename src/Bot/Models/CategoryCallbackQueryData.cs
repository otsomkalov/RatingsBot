using System.Collections.Immutable;
using Bot.Constants;
using Telegram.Bot.Types;

namespace Bot.Models;

public class CategoryCallbackQueryData : CallbackQueryData
{
    public CategoryCallbackQueryData(CallbackQuery query) : base(query)
    {
        var callbackData = query.Data.Split(ReplyMarkup.Separator).ToImmutableList();

        ItemId = int.Parse(callbackData[0]);

        if (callbackData.Count == 4)
        {
            Page = int.Parse(callbackData[2]);
            CategoryId = int.TryParse(callbackData[3], out var id) ? id : 0;
        }
        else
        {
            Page = 0;
            CategoryId = 0;
        }
    }

    public int CategoryId { get; }

    public int Page { get; }
}