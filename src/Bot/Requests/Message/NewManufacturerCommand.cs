using TG = Telegram.Bot.Types;

namespace Bot.Requests.Message;

public record NewManufacturerCommand(TG.Message Message) : IRequest;