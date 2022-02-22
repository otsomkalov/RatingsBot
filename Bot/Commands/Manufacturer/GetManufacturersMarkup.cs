using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Commands.Manufacturer;

public record GetManufacturersMarkup(int ItemId) : IRequest<InlineKeyboardMarkup>;