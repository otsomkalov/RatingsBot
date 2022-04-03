using System.Collections.ObjectModel;
using Bot.Constants;
using Bot.Requests.InlineKeyboardMarkup;
using Core.Data;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Handlers.InlineKeyboardMarkup;

public class GetInlineKeyboardMarkupHandler : IRequestHandler<GetInlineKeyboardMarkup, Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup>
{
    private readonly IAppDbContext _context;
    private readonly IMediator _mediator;

    public GetInlineKeyboardMarkupHandler(IAppDbContext context, IMediator mediator)
    {
        _mediator = mediator;
        _context = context;
    }

    public async Task<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup> Handle(GetInlineKeyboardMarkup request,
        CancellationToken cancellationToken)
    {
        var (itemId, type, page) = request;

        var (buttonsPerPage, buttonsPerRow) = await _mediator.Send(new GetKeyboardSettings(), cancellationToken);

        IQueryable<NamedEntity> set = type switch
        {
            ReplyMarkup.Category => _context.Categories,
            ReplyMarkup.Manufacturer => _context.Manufacturers,
            ReplyMarkup.Place => _context.Places
        };

        var entities = await set
            .OrderByDescending(c => c.Id)
            .Skip(page * buttonsPerPage)
            .Take(buttonsPerPage + 1)
            .ToListAsync(cancellationToken);

        var rows = new List<IEnumerable<InlineKeyboardButton>>();

        var maximumButtons = entities.Count > buttonsPerPage
            ? buttonsPerPage
            : entities.Count;

        for (var i = 0; i < maximumButtons; i += buttonsPerRow)
        {
            var buttons = entities.Skip(i)
                .Take(buttonsPerRow)
                .Select(entity => new InlineKeyboardButton(entity.Name)
                {
                    CallbackData = string.Join(ReplyMarkup.Separator, itemId, type, page, entity.Id)
                });

            rows.Add(buttons);
        }

        if (type is ReplyMarkup.Place or ReplyMarkup.Manufacturer)
        {
            var noneRow = new Collection<InlineKeyboardButton>();

            noneRow.Add(new(ReplyMarkup.NoneButtonText)
            {
                CallbackData = string.Join(ReplyMarkup.Separator, itemId, type, page, null)
            });

            rows.Add(noneRow);
        }

        var lastRow = new Collection<InlineKeyboardButton>();

        if (page != 0)
        {
            lastRow.Add(new(ReplyMarkup.PreviousPageButtonText)
            {
                CallbackData = string.Join(ReplyMarkup.Separator, itemId, type, page - 1, 0)
            });
        }

        lastRow.Add(new(ReplyMarkup.RefreshButtonText)
        {
            CallbackData = string.Join(ReplyMarkup.Separator, itemId, type, page, 0)
        });

        if (entities.Count > buttonsPerPage)
        {
            lastRow.Add(new(ReplyMarkup.NextPageButtonText)
            {
                CallbackData = string.Join(ReplyMarkup.Separator, itemId, type, page + 1, 0)
            });
        }

        rows.Add(lastRow);

        return new(rows);
    }
}