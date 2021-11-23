using TG = Telegram.Bot.Types;

namespace Bot.Commands.Message;

public record StartCommand(TG.Message Message) : MessageCommand(Message);