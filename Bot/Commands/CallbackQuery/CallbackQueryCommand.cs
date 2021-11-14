using RatingsBot.Models;

namespace RatingsBot.Commands.CallbackQuery;

public record CallbackQueryCommand(Telegram.Bot.Types.CallbackQuery CallbackQuery, int? EntityId, Item Item) : IRequest;