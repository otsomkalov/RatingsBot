using Bot.Models;

namespace Bot.Requests.CallbackQuery;

public record ProcessRatingCommand(RatingCallbackQueryData CallbackQueryData) : IRequest;