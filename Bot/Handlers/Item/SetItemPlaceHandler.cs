using Bot.Commands.Item;
using Bot.Commands.Place;
using Bot.Commands.Rating;
using Microsoft.EntityFrameworkCore;

namespace Bot.Handlers.Item;

public class SetItemPlaceHandler : AsyncRequestHandler<SetItemPlace>
{
    private readonly ITelegramBotClient _bot;
    private readonly AppDbContext _context;
    private readonly IMediator _mediator;

    public SetItemPlaceHandler(ITelegramBotClient bot, AppDbContext context, IMediator mediator)
    {
        _bot = bot;
        _context = context;
        _mediator = mediator;
    }

    protected override async Task Handle(SetItemPlace request, CancellationToken cancellationToken)
    {
        var (callbackQuery, entityId, itemId) = request;

        if (entityId is -1)
        {
            var placesMarkup = await _mediator.Send(new GetPlacesMarkup(itemId), cancellationToken);

            await _bot.EditMessageReplyMarkupAsync(new(callbackQuery.From.Id),
                callbackQuery.Message.MessageId,
                placesMarkup, cancellationToken);
        }
        else
        {
            var item = await _context.Items
                .Include(i => i.Category)
                .Include(i => i.Place)
                .FirstOrDefaultAsync(i => i.Id == itemId, cancellationToken);

            item.PlaceId = entityId;

            var entityEntry = _context.Items.Update(item);
            await _context.SaveChangesAsync(cancellationToken);

            await entityEntry.Reference(i => i.Place).LoadAsync(cancellationToken);

            var ratingsMarkup = await _mediator.Send(new GetRatingsMarkup(itemId), cancellationToken);
            var messageText = await _mediator.Send(new GetItemMessageText(item), cancellationToken);

            await _bot.EditMessageTextAsync(new(callbackQuery.From.Id),
                callbackQuery.Message.MessageId,
                messageText,
                replyMarkup: ratingsMarkup,
                cancellationToken: cancellationToken);
        }
    }
}