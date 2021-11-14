using Microsoft.Extensions.Localization;
using RatingsBot.Commands.CallbackQuery;
using RatingsBot.Constants;
using RatingsBot.Resources;

namespace RatingsBot.Handlers.CallbackQuery;

public class SetCategoryCommandHandler : AsyncRequestHandler<SetCategoryCommand>
{
    private readonly CategoryService _categoryService;
    private readonly ITelegramBotClient _bot;
    private readonly ItemService _itemService;
    private readonly PlaceService _placeService;
    private readonly IStringLocalizer<Messages> _localizer;

    public SetCategoryCommandHandler(CategoryService categoryService, ITelegramBotClient bot, ItemService itemService,
        PlaceService placeService, IStringLocalizer<Messages> localizer)
    {
        _categoryService = categoryService;
        _bot = bot;
        _itemService = itemService;
        _placeService = placeService;
        _localizer = localizer;
    }

    protected override async Task Handle(SetCategoryCommand request, CancellationToken cancellationToken)
    {
        var (callbackQuery, entityId, item) = request;

        if (!entityId.HasValue)
        {
            var categories = await _categoryService.ListAsync();

            await _bot.EditMessageReplyMarkupAsync(new(callbackQuery.From.Id),
                callbackQuery.Message.MessageId,
                ReplyMarkupHelpers.GetCategoriesMarkup(item.Id, categories), cancellationToken);
        }
        else
        {
            await _itemService.UpdateCategoryAsync(item, entityId.Value);

            var places = await _placeService.ListAsync();

            await _bot.EditMessageTextAsync(new(callbackQuery.From.Id),
                callbackQuery.Message.MessageId,
                _localizer[ResourcesNames.Place],
                replyMarkup: ReplyMarkupHelpers.GetPlacesMarkup(item.Id, places), cancellationToken: cancellationToken);
        }
    }
}