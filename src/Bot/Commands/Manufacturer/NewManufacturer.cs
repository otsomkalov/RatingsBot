using TG = Telegram.Bot.Types;

namespace Bot.Commands.Manufacturer;

public record NewManufacturer(TG.Message Message) : IRequest;