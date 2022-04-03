using Bot.Models;

namespace Bot.Requests.CallbackQuery;

public record ProcessCategoryCommand(CategoryCallbackQueryData CallbackQueryQueryData) : IRequest;