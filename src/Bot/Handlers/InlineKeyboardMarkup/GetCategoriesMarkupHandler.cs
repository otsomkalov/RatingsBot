using System.Collections.ObjectModel;
using Bot.Constants;
using Bot.Requests.InlineKeyboardMarkup;
using Core.Data;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Handlers.InlineKeyboardMarkup;

public class GetCategoriesMarkupHandler : IRequestHandler<GetCategoriesMarkup, Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup>
{
    private readonly IAppDbContext _context;
    private readonly IMediator _mediator;

    public GetCategoriesMarkupHandler(IAppDbContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup> Handle(GetCategoriesMarkup request,
        CancellationToken cancellationToken)
    {
        var (itemId, page) = request;

        var (buttonsPerPage, buttonsPerRow) = await _mediator.Send(new GetKeyboardSettings(), cancellationToken);

        var categories = await _context.Categories
            .OrderByDescending(c => c.Id)
            .Skip(page * buttonsPerPage)
            .Take(buttonsPerPage + 1)
            .ToListAsync(cancellationToken);

        var rows = new List<IEnumerable<InlineKeyboardButton>>();

        var maximumButtons = categories.Count > buttonsPerPage
            ? buttonsPerPage
            : categories.Count;

        for (var i = 0; i < maximumButtons; i += buttonsPerRow)
        {
            var buttons = categories.Skip(i)
                .Take(buttonsPerRow)
                .Select(category => new InlineKeyboardButton(category.Name)
                {
                    CallbackData = string.Join(ReplyMarkup.Separator, itemId, ReplyMarkup.Category, page, category.Id)
                });

            rows.Add(buttons);
        }

        var lastRow = new Collection<InlineKeyboardButton>();

        if (page != 0)
        {
            lastRow.Add(new(ReplyMarkup.PreviousPageButtonText)
            {
                CallbackData = string.Join(ReplyMarkup.Separator, itemId, ReplyMarkup.Category, page - 1, 0)
            });
        }

        lastRow.Add(new(ReplyMarkup.RefreshButtonText)
        {
            CallbackData = string.Join(ReplyMarkup.Separator, itemId, ReplyMarkup.Category, page, 0)
        });

        if (categories.Count > buttonsPerPage)
        {
            lastRow.Add(new(ReplyMarkup.NextPageButtonText)
            {
                CallbackData = string.Join(ReplyMarkup.Separator, itemId, ReplyMarkup.Category, page + 1, 0)
            });
        }

        rows.Add(lastRow);

        return new(rows);
    }
}