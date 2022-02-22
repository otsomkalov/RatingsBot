using TG = Telegram.Bot.Types;

namespace Bot.Commands.Item;

public record SetItemManufacturer(TG.CallbackQuery CallbackQuery, int? EntityId, int Item) : IRequest;