using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Commands.Category;

public record GetCategoriesMarkup(int ItemId) : IRequest<InlineKeyboardMarkup>;