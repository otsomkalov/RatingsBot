using TG = Telegram.Bot.Types;

namespace RatingsBot.Commands.Message;

public record StartCommand(TG.Message Message) : MessageCommand(Message);