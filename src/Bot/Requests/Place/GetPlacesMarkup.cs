using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Requests.Place;

public record GetPlacesMarkup(int ItemId) : IRequest<InlineKeyboardMarkup>;