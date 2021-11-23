using TG = Telegram.Bot.Types;

namespace Bot.Commands.Message;

public record MessageCommand(TG.Message Message) : IRequest;