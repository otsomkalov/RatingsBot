using TG = Telegram.Bot.Types;

namespace Bot.Requests.Message;

public record NewItemCommand(TG.Message Message) : IRequest;