namespace RatingsBot.Commands.Message;

public record NewItemCommand(Telegram.Bot.Types.Message Message) : MessageCommand(Message);