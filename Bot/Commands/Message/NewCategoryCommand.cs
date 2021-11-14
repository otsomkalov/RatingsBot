namespace RatingsBot.Commands.Message;

public record NewCategoryCommand(Telegram.Bot.Types.Message Message) : MessageCommand(Message);