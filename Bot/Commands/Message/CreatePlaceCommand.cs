using TG = Telegram.Bot.Types;

namespace RatingsBot.Commands.Message;

public record CreatePlaceCommand(TG.Message Message) : MessageCommand(Message);