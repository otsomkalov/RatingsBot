namespace Bot.Requests.InlineKeyboardMarkup;

public record GetRatingsMarkup(int ItemId) : IRequest<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup>;