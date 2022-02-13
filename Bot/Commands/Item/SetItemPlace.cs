using TG = Telegram.Bot.Types;

namespace Bot.Commands.Item;

public record SetItemPlace(TG.CallbackQuery CallbackQuery, int? EntityId, int Item) : IRequest;