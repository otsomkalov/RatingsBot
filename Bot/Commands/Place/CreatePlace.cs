using TG = Telegram.Bot.Types;

namespace Bot.Commands.Place;

public record CreatePlace(TG.Message Message) : IRequest;