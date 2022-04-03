using Bot.Models;

namespace Bot.Requests.CallbackQuery;

public record ProcessCategoryCommand(EntitiesCallbackQueryData CallbackQueryData) : IRequest;