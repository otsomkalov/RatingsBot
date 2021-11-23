using TG = Telegram.Bot.Types;

namespace RatingsBot.Commands.Message;

public record NewItemCommand(TG.Message Message) : MessageCommand(Message);