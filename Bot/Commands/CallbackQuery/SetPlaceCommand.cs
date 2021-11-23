using RatingsBot.Models;
using TG = Telegram.Bot.Types;

namespace RatingsBot.Commands.CallbackQuery;

public record SetPlaceCommand(TG.CallbackQuery CallbackQuery, int? EntityId, Item Item) :
    CallbackQueryCommand(CallbackQuery, EntityId, Item);