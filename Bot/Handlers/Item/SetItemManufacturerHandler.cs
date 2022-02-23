using Bot.Commands.Item;
using Bot.Commands.Manufacturer;
using Bot.Commands.Place;
using Bot.Resources;
using Microsoft.Extensions.Localization;

namespace Bot.Handlers.Item;

public class SetItemManufacturerHandler : AsyncRequestHandler<SetItemManufacturer>
{
    private readonly ITelegramBotClient _bot;
    private readonly AppDbContext _context;
    private readonly IStringLocalizer<Messages> _localizer;
    private readonly IMediator _mediator;

    public SetItemManufacturerHandler(IMediator mediator, ITelegramBotClient bot, AppDbContext context,
        IStringLocalizer<Messages> localizer)
    {
        _mediator = mediator;
        _bot = bot;
        _context = context;
        _localizer = localizer;
    }

    protected override async Task Handle(SetItemManufacturer request, CancellationToken cancellationToken)
    {
        var (callbackQuery, entityId, itemId) = request;

        if (entityId is -1)
        {
            var manufacturersMarkup = await _mediator.Send(new GetManufacturersMarkup(itemId), cancellationToken);

            await _bot.EditMessageReplyMarkupAsync(new(callbackQuery.From.Id),
                callbackQuery.Message.MessageId,
                manufacturersMarkup,
                cancellationToken);

            return;
        }

        if (entityId.HasValue)
        {
            var item = await _context.Items.FindAsync(itemId);

            item.ManufacturerId = entityId;

            _context.Items.Update(item);
            await _context.SaveChangesAsync(cancellationToken);
        }

        var placesMarkup = await _mediator.Send(new GetPlacesMarkup(itemId), cancellationToken);

        await _bot.EditMessageTextAsync(new(callbackQuery.From.Id),
            callbackQuery.Message.MessageId,
            _localizer[nameof(Messages.SelectPlace)],
            replyMarkup: placesMarkup,
            cancellationToken: cancellationToken);
    }
}