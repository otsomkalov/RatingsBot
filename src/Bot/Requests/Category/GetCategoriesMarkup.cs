using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Requests.Category;

public record GetCategoriesMarkup(int ItemId) : IRequest<InlineKeyboardMarkup>;