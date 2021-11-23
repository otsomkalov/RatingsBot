using TG = Telegram.Bot.Types;

namespace RatingsBot.Commands.Message;

public record CreateItemCommand(TG.Message Message) : MessageCommand(Message);