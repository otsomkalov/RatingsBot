using Bot.Models;

namespace Bot.Requests.CallbackQuery;

public record ProcessPlaceCommand(CallbackQueryData CallbackQueryData) : IRequest;