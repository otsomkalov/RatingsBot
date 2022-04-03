using Telegram.Bot.Types;

namespace Bot.Models;

public abstract class CallbackQueryData
{
    protected CallbackQueryData(CallbackQuery query)
    {
        MessageId = query.Message?.MessageId;
        InlineMessageId = query.InlineMessageId;
        UserId = query.From.Id;
        QueryId = query.Id;
    }

    public int? EntityId { get; protected init; }

    public string InlineMessageId { get; }

    public int ItemId { get; protected init; }

    public int? MessageId { get; }

    public string QueryId { get; }

    public long UserId { get; }
}