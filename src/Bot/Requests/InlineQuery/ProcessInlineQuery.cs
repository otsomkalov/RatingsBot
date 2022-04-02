namespace Bot.Requests.InlineQuery;

public record ProcessInlineQuery(Telegram.Bot.Types.InlineQuery InlineQuery) : IRequest;