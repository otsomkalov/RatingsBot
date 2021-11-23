using TG = Telegram.Bot.Types;

namespace Bot.Commands.Message;

public record NewPlaceCommand(TG.Message Message) : MessageCommand(Message);