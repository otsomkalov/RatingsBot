namespace Bot.Requests.CallbackQuery;

public record ProcessCallbackQuery(Telegram.Bot.Types.CallbackQuery CallbackQuery) : IRequest<Unit>;