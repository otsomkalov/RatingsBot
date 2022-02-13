using TG = Telegram.Bot.Types;

namespace Bot.Commands.Place;

public record NewPlace(TG.Message Message) : IRequest;