using Bot.Commands.Category;
using Bot.Commands.Item;
using Bot.Commands.Place;
using Bot.Resources;
using Microsoft.Extensions.Localization;

namespace Bot.Handlers.Item;

public class SetItemCategoryHandler : AsyncRequestHandler<SetItemCategory>
{
    private readonly ITelegramBotClient _bot;
    private readonly AppDbContext _context;
    private readonly IStringLocalizer<Messages> _localizer;
    private readonly IMediator _mediator;

    public SetItemCategoryHandler(ITelegramBotClient bot, IStringLocalizer<Messages> localizer, AppDbContext context, IMediator mediator)
    {
        _bot = bot;
        _localizer = localizer;
        _context = context;
        _mediator = mediator;
    }

    protected override async Task Handle(SetItemCategory request, CancellationToken cancellationToken)
    {
        var (callbackQuery, entityId, itemId) = request;

        if (!entityId.HasValue)
        {
            var categoriesMarkup = await _mediator.Send(new GetCategoriesMarkup(itemId), cancellationToken);

            await _bot.EditMessageReplyMarkupAsync(new(callbackQuery.From.Id),
                callbackQuery.Message.MessageId,
                categoriesMarkup,
                cancellationToken);
        }
        else
        {
            var item = await _context.Items.FindAsync(itemId);

            item.CategoryId = entityId.Value;

            _context.Items.Update(item);
            await _context.SaveChangesAsync(cancellationToken);

            var placesMarkup = await _mediator.Send(new GetPlacesMarkup(itemId), cancellationToken);

            await _bot.EditMessageTextAsync(new(callbackQuery.From.Id),
                callbackQuery.Message.MessageId,
                _localizer[nameof(Messages.SelectPlace)],
                replyMarkup: placesMarkup,
                cancellationToken: cancellationToken);
        }
    }
}