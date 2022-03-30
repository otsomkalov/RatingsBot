using System.Collections.Immutable;
using Bot.Constants;
using Telegram.Bot.Types;

namespace Bot.Models;

public record CallbackQueryData
{
    public CallbackQueryData(CallbackQuery query)
    {
        var callbackData = query.Data.Split(ReplyMarkup.Separator).ToImmutableList();

        ItemId = int.Parse(callbackData[0]);
        EntityId = int.TryParse(callbackData[2], out var id) ? id : null;
        Command = callbackData[1];
        MessageId = query.Message?.MessageId;
        InlineMessageId = query.InlineMessageId;
        UserId = query.From.Id;
        QueryId = query.Id;
    }

    public string Command { get; }

    public int? EntityId { get; }

    public string InlineMessageId { get; }

    public int ItemId { get; }

    public int? MessageId { get; }

    public string QueryId { get; }

    public long UserId { get; }
}