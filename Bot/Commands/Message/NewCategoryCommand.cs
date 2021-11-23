using TG = Telegram.Bot.Types;

namespace RatingsBot.Commands.Message;

public record NewCategoryCommand(TG.Message Message) : MessageCommand(Message);