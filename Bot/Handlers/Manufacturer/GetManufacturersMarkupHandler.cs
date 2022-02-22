using Bot.Commands.Manufacturer;
using Bot.Constants;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Handlers.Manufacturer;

public class GetManufacturersMarkupHandler : IRequestHandler<GetManufacturersMarkup, InlineKeyboardMarkup>
{
    private readonly AppDbContext _context;

    public GetManufacturersMarkupHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<InlineKeyboardMarkup> Handle(GetManufacturersMarkup request, CancellationToken cancellationToken)
    {
        var itemId = request.ItemId;

        var manufacturers = await _context.Manufacturers.AsNoTracking().ToListAsync(cancellationToken);

        var rows = new List<IEnumerable<InlineKeyboardButton>>();

        for (var i = 0; i < manufacturers.Count; i += ReplyMarkup.Columns)
        {
            var buttons = manufacturers.Skip(i)
                .Take(ReplyMarkup.Columns)
                .Select(m => new InlineKeyboardButton
                {
                    Text = m.Name,
                    CallbackData = string.Join(ReplyMarkup.Separator, itemId, ReplyMarkup.Manufacturer, m.Id)
                });

            rows.Add(buttons);
        }

        rows.Add(new InlineKeyboardButton[]
        {
            new()
            {
                Text = "<None>",
                CallbackData = string.Join(ReplyMarkup.Separator, itemId, ReplyMarkup.Manufacturer, null)
            },
            new()
            {
                Text = "Refresh",
                CallbackData = string.Join(ReplyMarkup.Separator, itemId, ReplyMarkup.Manufacturer, -1)
            }
        });

        return new(rows);
    }
}