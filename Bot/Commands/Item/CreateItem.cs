using TG = Telegram.Bot.Types;

namespace Bot.Commands.Item;

public record CreateItem(TG.Message Message) : IRequest;