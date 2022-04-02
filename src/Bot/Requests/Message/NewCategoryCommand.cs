using TG = Telegram.Bot.Types;

namespace Bot.Requests.Message;

public record NewCategoryCommand(TG.Message Message) : IRequest;