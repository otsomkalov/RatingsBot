namespace Bot.Requests.InlineKeyboardMarkup;

public record GetPlacesMarkup(int ItemId) : IRequest<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup>;