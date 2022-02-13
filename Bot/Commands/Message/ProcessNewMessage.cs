namespace Bot.Commands.Message;

public record ProcessNewMessage(Telegram.Bot.Types.Message Message) : IRequest;