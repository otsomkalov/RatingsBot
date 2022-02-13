using Bot.Commands.Category;
using Bot.Commands.Item;
using Bot.Resources;
using Microsoft.Extensions.Localization;

namespace Bot.Handlers.Item;

public class CreateItemHandler : AsyncRequestHandler<CreateItem>
{
    private readonly ITelegramBotClient _bot;
    private readonly IStringLocalizer<Messages> _localizer;
    private readonly AppDbContext _context;
    private readonly IMediator _mediator;

    public CreateItemHandler(ITelegramBotClient bot, IStringLocalizer<Messages> localizer, AppDbContext context, IMediator mediator)
    {
        _bot = bot;
        _localizer = localizer;
        _context = context;
        _mediator = mediator;
    }

    protected override async Task Handle(CreateItem request, CancellationToken cancellationToken)
    {
        var message = request.Message;

        var newItem = new Models.Item
        {
            Name = message.Text.Trim()
        };

        await _context.AddAsync(newItem, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var categoriesMarkup = await _mediator.Send(new GetCategoriesMarkup(newItem.Id), cancellationToken);

        await _bot.SendTextMessageAsync(new(message.From.Id),
            _localizer[nameof(Messages.SelectCategory)],
            replyMarkup: categoriesMarkup,
            replyToMessageId: message.MessageId,
            cancellationToken: cancellationToken);
    }
}