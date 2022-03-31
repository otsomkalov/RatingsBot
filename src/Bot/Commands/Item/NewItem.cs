using TG = Telegram.Bot.Types;

namespace Bot.Commands.Item;

public record NewItem(TG.Message Message) : IRequest;