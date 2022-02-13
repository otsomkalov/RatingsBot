using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Commands.Rating;

public record GetRatingsMarkup(int ItemId) : IRequest<InlineKeyboardMarkup>;