using Bot.Models;

namespace Bot.Requests.CallbackQuery;

public record ProcessManufacturerCommand(EntitiesCallbackQueryData CallbackQueryData) : IRequest;