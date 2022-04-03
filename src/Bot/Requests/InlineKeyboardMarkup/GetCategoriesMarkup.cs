namespace Bot.Requests.InlineKeyboardMarkup;

public record GetCategoriesMarkup(int ItemId, int Page = 0) : IRequest<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup>;