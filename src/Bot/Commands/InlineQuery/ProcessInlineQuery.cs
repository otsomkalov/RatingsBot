namespace Bot.Commands.InlineQuery;

public record ProcessInlineQuery(Telegram.Bot.Types.InlineQuery InlineQuery) : IRequest;