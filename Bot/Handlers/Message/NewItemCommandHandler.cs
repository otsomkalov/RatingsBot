using Microsoft.Extensions.Localization;
using RatingsBot.Commands.Message;
using RatingsBot.Constants;
using RatingsBot.Resources;

namespace RatingsBot.Handlers.Message;

public class NewItemCommandHandler : AsyncRequestHandler<NewItemCommand>
{
    private readonly ITelegramBotClient _bot;
    private readonly IStringLocalizer<Messages> _localizer;
    private readonly CategoryService _categoryService;
    private readonly ItemService _itemService;

    public NewItemCommandHandler(ITelegramBotClient bot, IStringLocalizer<Messages> localizer, CategoryService categoryService, ItemService itemService)
    {
        _bot = bot;
        _localizer = localizer;
        _categoryService = categoryService;
        _itemService = itemService;
    }

    protected override async Task Handle(NewItemCommand request, CancellationToken cancellationToken)
    {
        var message = request.Message;

        if (message.Text.Length == Constants.Commands.NewItem.Length)
        {
            await _bot.SendTextMessageAsync(new(message.From.Id),
                _localizer[ResourcesNames.NewItemCommand], cancellationToken: cancellationToken);

            return;
        }

        var categories = await _categoryService.ListAsync();

        var newItemId = await _itemService.AddAsync(message.Text[Constants.Commands.NewItem.Length..].Trim());

        await _bot.SendTextMessageAsync(new(message.From.Id),
            _localizer[ResourcesNames.Category],
            replyMarkup: ReplyMarkupHelpers.GetCategoriesMarkup(newItemId, categories), cancellationToken: cancellationToken);
    }
}