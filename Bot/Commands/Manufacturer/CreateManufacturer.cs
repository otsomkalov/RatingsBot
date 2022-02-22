using TG = Telegram.Bot.Types;

namespace Bot.Commands.Manufacturer;

public record CreateManufacturer(TG.Message Message) : IRequest;