using RatingsBot.Models;
using TG = Telegram.Bot.Types;

namespace RatingsBot.Commands.CallbackQuery;

public record CallbackQueryCommand(TG.CallbackQuery CallbackQuery, int? EntityId, Item Item) : IRequest;