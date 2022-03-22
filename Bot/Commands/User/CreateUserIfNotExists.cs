namespace Bot.Commands.User;

public record CreateUserIfNotExists(Telegram.Bot.Types.User User) : IRequest;