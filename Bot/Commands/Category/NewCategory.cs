using TG = Telegram.Bot.Types;

namespace Bot.Commands.Category;

public record NewCategory(TG.Message Message) : IRequest;