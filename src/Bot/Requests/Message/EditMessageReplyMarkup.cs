using Bot.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Requests.Message;

public record EditMessageReplyMarkup(CallbackQueryData CallbackQueryData, InlineKeyboardMarkup InlineKeyboardMarkup) : IRequest;