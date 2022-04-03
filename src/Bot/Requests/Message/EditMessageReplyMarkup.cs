using Bot.Models;

namespace Bot.Requests.Message;

public record EditMessageReplyMarkup(CallbackQueryData CallbackQueryData,
    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup InlineKeyboardMarkup) : IRequest;