using TG = Telegram.Bot.Types;

namespace Bot.Requests.Message;

public record NewPlaceCommand(TG.Message Message) : IRequest;