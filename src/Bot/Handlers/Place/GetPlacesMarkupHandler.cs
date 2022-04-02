using Bot.Constants;
using Bot.Requests.Place;
using Data;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Handlers.Place;

public class GetPlacesMarkupHandler : IRequestHandler<GetPlacesMarkup, InlineKeyboardMarkup>
{
    private readonly AppDbContext _context;

    public GetPlacesMarkupHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<InlineKeyboardMarkup> Handle(GetPlacesMarkup request, CancellationToken cancellationToken)
    {
        var itemId = request.ItemId;
        var places = await _context.Places.ToListAsync(cancellationToken);

        var rows = new List<IEnumerable<InlineKeyboardButton>>();

        for (var i = 0; i < places.Count; i += ReplyMarkup.Columns)
        {
            var buttons = places.Skip(i)
                .Take(ReplyMarkup.Columns)
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