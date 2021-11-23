using TG = Telegram.Bot.Types;
using RatingsBot.Models;

namespace RatingsBot.Commands.CallbackQuery;

public record SetCategoryCommand(TG.CallbackQuery CallbackQuery, int? EntityId, Item Item) :
    CallbackQueryCommand(CallbackQuery, EntityId, Item);