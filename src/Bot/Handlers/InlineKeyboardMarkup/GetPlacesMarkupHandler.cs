using Bot.Constants;
using Bot.Requests.InlineKeyboardMarkup;
using Data;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Handlers.InlineKeyboardMarkup;

public class GetPlacesMarkupHandler : IRequestHandler<GetPlacesMarkup, Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup>
{
    private readonly AppDbContext _context;

    public GetPlacesMarkupHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup> Handle(GetPlacesMarkup request,
        CancellationToken cancellationToken)
    {
        var itemId = request.ItemId;
        var places = await _context.Places.ToListAsync(cancellationToken);

        var rows = new List<IEnumerable<InlineKeyboardButton>>();

        for (var i = 0; i < places.Count; i += ReplyMarkup.ButtonsPerRow)
        {
            var buttons = places.Skip(i)
                .Take(ReplyMarkup.ButtonsPerRow)
                .Select(place => new InlineKeyboardButton(place.Name)
                {
                    CallbackData = string.Join(ReplyMarkup.Separator, itemId, ReplyMarkup.Place, place.Id)
                });

            rows.Add(buttons);
        }

        rows.Add(new InlineKeyboardButton[]
        {
            new("<None>")
            {
                CallbackData = string.Join(ReplyMarkup.Separator, itemId, ReplyMarkup.Place, null)
            },
            new("Refresh")
            {
                CallbackData = string.Join(ReplyMarkup.Separator, itemId, ReplyMarkup.Place, 0)
            }
        });

        return new(rows);
    }
}