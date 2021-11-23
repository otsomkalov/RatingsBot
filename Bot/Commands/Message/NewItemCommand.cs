using TG = Telegram.Bot.Types;

namespace Bot.Commands.Message;

public record NewItemCommand(TG.Message Message) : MessageCommand(Message);