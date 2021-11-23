using TG = Telegram.Bot.Types;

namespace Bot.Commands.Message;

public record CreateItemCommand(TG.Message Message) : MessageCommand(Message);