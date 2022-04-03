namespace Bot.Requests.InlineKeyboardMarkup;

public record GetInlineKeyboardMarkup
    (int ItemId, string Type, int Page = 0) : IRequest<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup>;