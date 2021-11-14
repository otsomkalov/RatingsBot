using MediatR;

namespace RatingsBot.Commands.Message;

public record MessageCommand(Telegram.Bot.Types.Message Message) : IRequest;