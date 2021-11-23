using TG = Telegram.Bot.Types;

namespace Bot.Commands.Message;

public record NewCategoryCommand(TG.Message Message) : MessageCommand(Message);