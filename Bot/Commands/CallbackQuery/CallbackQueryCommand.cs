using Bot.Models;
using TG = Telegram.Bot.Types;

namespace Bot.Commands.CallbackQuery;

public record CallbackQueryCommand(TG.CallbackQuery CallbackQuery, int? EntityId, Item Item) : IRequest;