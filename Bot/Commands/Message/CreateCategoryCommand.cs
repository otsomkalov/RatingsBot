using TG = Telegram.Bot.Types;

namespace RatingsBot.Commands.Message;

public record CreateCategoryCommand(TG.Message Message) : MessageCommand(Message);