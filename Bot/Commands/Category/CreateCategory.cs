using TG = Telegram.Bot.Types;

namespace Bot.Commands.Category;

public record CreateCategory(TG.Message Message) : IRequest;