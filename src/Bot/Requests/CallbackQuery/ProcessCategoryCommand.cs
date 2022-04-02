using Bot.Models;

namespace Bot.Requests.CallbackQuery;

public record ProcessCategoryCommand(CallbackQueryData CallbackQueryData) : IRequest;