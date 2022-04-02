namespace Bot.Requests.Message;

public record ProcessMessage(Telegram.Bot.Types.Message Message) : IRequest;