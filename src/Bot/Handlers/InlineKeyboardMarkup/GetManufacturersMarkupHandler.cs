using Bot.Constants;
using Bot.Requests.InlineKeyboardMarkup;
using Data;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Handlers.InlineKeyboardMarkup;

public class GetManufacturersMarkupHandler : IRequestHandler<GetManufacturersMarkup, Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup>
{
    private readonly AppDbContext _context;

    public GetManufacturersMarkupHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup> Handle(GetManufacturersMarkup request,
        CancellationToken cancellationToken)
    {
        var itemId = request.ItemId;

        var manufacturers = await _context.Manufacturers.ToListAsync(cancellationToken);

        var rows = new List<IEnumerable<InlineKeyboardButton>>();

        for (var i = 0; i < manufacturers.Count; i += ReplyMarkup.ButtonsPerRow)
        {
            var buttons = manufacturers.Skip(i)
                .Take(ReplyMarkup.ButtonsPerRow)
                .Select(manufacturer => new InlineKeyboardButton(manufacturer.Name)
                {
                    CallbackData = string.Join(ReplyMarkup.Separator, itemId, ReplyMarkup.Manufacturer, manufacturer.Id)
                });

            rows.Add(buttons);
        }

        rows.Add(new InlineKeyboardButton[]
        {
            new("<None>")
            {
                CallbackData = string.Join(ReplyMarkup.Separator, itemId, ReplyMarkup.Manufacturer, null)
            },
            new("Refresh")
            {
                CallbackData = string.Join(ReplyMarkup.Separator, itemId, ReplyMarkup.Manufacturer, 0)
            }
        });

        return new(rows);
    }
}