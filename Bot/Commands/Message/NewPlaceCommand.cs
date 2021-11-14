namespace RatingsBot.Commands.Message;

public record NewPlaceCommand(Telegram.Bot.Types.Message Message) : MessageCommand(Message);