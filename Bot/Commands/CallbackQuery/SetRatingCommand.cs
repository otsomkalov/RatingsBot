using RatingsBot.Models;

namespace RatingsBot.Commands.CallbackQuery;

public record SetRatingCommand(Telegram.Bot.Types.CallbackQuery CallbackQuery, int? EntityId, Item Item) :
    CallbackQueryCommand(CallbackQuery, EntityId, Item);