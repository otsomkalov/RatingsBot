using Bot.Commands.Category;
using Bot.Constants;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Handlers.Category;

public class GetCategoriesMarkupHandler : IRequestHandler<GetCategoriesMarkup, InlineKeyboardMarkup>
{
    private readonly AppDbContext _context;

    public GetCategoriesMarkupHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<InlineKeyboardMarkup> Handle(GetCategoriesMarkup request, CancellationToken cancellationToken)
    {
        var itemId = request.ItemId;

        var categories = await _context.Categories.AsNoTracking().ToListAsync(cancellationToken);

        var rows = new List<IEnumerable<InlineKeyboardButton>>();

        for (var i = 0; i < categories.Count; i += ReplyMarkup.Columns)
        {
            var buttons = categories.Skip(i)
                .Take(ReplyMarkup.Columns)
                .Select(c => new InlineKeyboardButton
                {
                    Text = c.Name,
                    CallbackData = string.Join(ReplyMarkup.Separator, itemId, ReplyMarkup.Category, c.Id)
                });

            rows.Add(buttons);
        }

        rows.Add(new InlineKeyboardButton[]
        {
            new()
            {
                Text = "Refresh",
                CallbackData = string.Join(ReplyMarkup.Separator, itemId, ReplyMarkup.Category, null)
            }
        });

        return new(rows);
    }
}