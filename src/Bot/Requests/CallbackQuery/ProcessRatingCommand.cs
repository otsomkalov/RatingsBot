using Bot.Models;

namespace Bot.Requests.CallbackQuery;

public record ProcessRatingCommand(CallbackQueryData CallbackQueryData) : IRequest;