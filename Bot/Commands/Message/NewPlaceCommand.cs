using TG = Telegram.Bot.Types;

namespace RatingsBot.Commands.Message;

public record NewPlaceCommand(TG.Message Message) : MessageCommand(Message);