namespace Bot.Requests.InlineKeyboardMarkup;

public record GetManufacturersMarkup(int ItemId) : IRequest<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup>;