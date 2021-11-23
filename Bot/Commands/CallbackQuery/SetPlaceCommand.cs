using Bot.Models;
using TG = Telegram.Bot.Types;

namespace Bot.Commands.CallbackQuery;

public record SetPlaceCommand(TG.CallbackQuery CallbackQuery, int? EntityId, Item Item) :
    CallbackQueryCommand(CallbackQuery, EntityId, Item);