using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Commands.Place;

public record GetPlacesMarkup(int ItemId) : IRequest<InlineKeyboardMarkup>;