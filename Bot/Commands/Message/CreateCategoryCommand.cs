using TG = Telegram.Bot.Types;

namespace Bot.Commands.Message;

public record CreateCategoryCommand(TG.Message Message) : MessageCommand(Message);