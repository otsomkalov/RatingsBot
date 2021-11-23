using TG = Telegram.Bot.Types;

namespace RatingsBot.Commands.Message;

public record MessageCommand(TG.Message Message) : IRequest;