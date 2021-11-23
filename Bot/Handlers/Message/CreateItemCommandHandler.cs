using Microsoft.Extensions.Localization;
using RatingsBot.Commands.Message;
using RatingsBot.Resources;

namespace RatingsBot.Handlers.Message;

public class CreateItemCommandHandler : AsyncRequestHandler<CreateItemCommand>
{
    private readonly ITelegramBotClient _bot;
    private readonly CategoryService _categoryService;
    private readonly ItemService _itemService;
    private readonly IStringLocalizer<Messages> _localizer;

    public CreateItemCommandHandler(ItemService itemService, CategoryService categoryService, ITelegramBotClient bot,
        IStringLocalizer<Messages> localizer)
    {
        _itemService = itemService;
        _categoryService = categoryService;
        _bot = bot;
        _localizer = localizer;
    }

    protected override async Task Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        var message = request.Message;

        var newItemId = await _itemService.AddAsync(message.Text.Trim());
        var categories = await _categoryService.ListAsync();

        await _bot.SendTextMessageAsync(new(message.From.Id),
            _localizer[Messages.SelectCategory],
            replyMarkup: ReplyMarkupHelpers.GetCategoriesMarkup(newItemId, categories),
            replyToMessageId: message.MessageId,
            cancellationToken: cancellationToken);
    }
}