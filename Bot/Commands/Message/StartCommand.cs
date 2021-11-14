namespace RatingsBot.Commands.Message;

public record StartCommand(Telegram.Bot.Types.Message Message) : MessageCommand(Message);