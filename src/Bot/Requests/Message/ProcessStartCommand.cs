using TG = Telegram.Bot.Types;

namespace Bot.Requests.Message;

public record ProcessStartCommand(TG.Message Message) : IRequest;