namespace Bot.Commands.CallbackQuery;

public record ProcessCallbackQuery(Telegram.Bot.Types.CallbackQuery CallbackQuery) : IRequest<Unit>;