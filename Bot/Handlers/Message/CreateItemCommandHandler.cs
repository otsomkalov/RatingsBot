using Bot.Commands.Message;
using Bot.Models;
using Bot.Resources;
using Microsoft.Extensions.Localization;

namespace Bot.Handlers.Message;

public class CreateItemCommandHandler : AsyncRequestHandler<CreateItemCommand>
{
    private readonly ITelegramBotClient _bot;
    private readonly CategoryService _categoryService;
    private readonly IStringLocalizer<Messages> _localizer;
    private readonly AppDbContext _context;

    public CreateItemCommandHandler(CategoryService categoryService, ITelegramBotClient bot,
        IStringLocalizer<Messages> localizer, AppDbContext context)
    {
        _categoryService = categoryService;
        _bot = bot;
        _localizer = localizer;
        _context = context;
    }

    protected override async Task Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        var message = request.Message;

        var newItem = new Item
        {
            Name = message.Text.Trim()
        };

        await _context.AddAsync(newItem, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var categories = await _categoryService.ListAsync();

        await _bot.SendTextMessageAsync(new(message.From.Id),
            _localizer[nameof(Messages.SelectCategory)],
            replyMarkup: ReplyMarkupHelpers.GetCategoriesMarkup(newItem.Id, categories),
            replyToMessageId: message.MessageId,
            cancellationToken: cancellationToken);
    }
}