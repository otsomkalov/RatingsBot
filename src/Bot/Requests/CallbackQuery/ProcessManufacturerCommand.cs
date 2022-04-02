using Bot.Models;

namespace Bot.Requests.CallbackQuery;

public record ProcessManufacturerCommand(CallbackQueryData CallbackQueryData) : IRequest;