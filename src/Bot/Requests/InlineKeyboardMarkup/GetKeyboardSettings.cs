namespace Bot.Requests.InlineKeyboardMarkup;

public record GetKeyboardSettings : IRequest<(int ButtonsPerPage, int ButtonsPerRow)>;