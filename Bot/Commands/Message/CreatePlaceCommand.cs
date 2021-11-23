using TG = Telegram.Bot.Types;

namespace Bot.Commands.Message;

public record CreatePlaceCommand(TG.Message Message) : MessageCommand(Message);