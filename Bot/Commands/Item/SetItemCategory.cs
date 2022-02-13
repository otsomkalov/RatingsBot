using TG = Telegram.Bot.Types;

namespace Bot.Commands.Item;

public record SetItemCategory(TG.CallbackQuery CallbackQuery, int? EntityId, int ItemId) : IRequest;