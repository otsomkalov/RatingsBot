using Bot.Constants;
using Bot.Requests.InlineKeyboardMarkup;

namespace Bot.Handlers.InlineKeyboardMarkup;

public class GetColumnsCountHandler : IRequestHandler<GetKeyboardSettings, (int, int)>
{
    public Task<(int, int)> Handle(GetKeyboardSettings request, CancellationToken cancellationToken)
    {
        return Task.FromResult((ReplyMarkup.ButtonsPerPage, ReplyMarkup.ButtonsPerRow));
    }
}