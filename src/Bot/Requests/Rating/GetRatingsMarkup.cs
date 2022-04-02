using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Requests.Rating;

public record GetRatingsMarkup(int ItemId) : IRequest<InlineKeyboardMarkup>;