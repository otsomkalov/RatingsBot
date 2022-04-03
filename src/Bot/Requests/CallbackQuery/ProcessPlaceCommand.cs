using Bot.Models;

namespace Bot.Requests.CallbackQuery;

public record ProcessPlaceCommand(EntitiesCallbackQueryData CallbackQueryData) : IRequest;