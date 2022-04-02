using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Requests.Manufacturer;

public record GetManufacturersMarkup(int ItemId) : IRequest<InlineKeyboardMarkup>;