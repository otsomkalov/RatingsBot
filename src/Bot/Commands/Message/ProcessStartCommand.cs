using TG = Telegram.Bot.Types;

namespace Bot.Commands.Message;

public record ProcessStartCommand(TG.Message Message) : IRequest;