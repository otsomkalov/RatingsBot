using Bot.Commands.CallbackQuery;
using Bot.Resources;
using Microsoft.Extensions.Localization;

namespace Bot.Handlers.CallbackQuery;

public class SetCategoryCommandHandler : AsyncRequestHandler<SetCategoryCommand>
{
    private readonly CategoryService _categoryService;
    private readonly ITelegramBotClient _bot;
    private readonly AppDbContext _context;
    private readonly PlaceService _placeService;
    private readonly IStringLocalizer<Messages> _localizer;

    public SetCategoryCommandHandler(CategoryService categoryService, ITelegramBotClient bot,
        PlaceService placeService, IStringLocalizer<Messages> localizer, AppDbContext context)
    {
        _categoryService = categoryService;
        _bot = bot;
        _placeService = placeService;
        _localizer = localizer;
        _context = context;
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
            item.CategoryId = entityId.Value;

            _context.Update (item);
            await _context.SaveChangesAsync(cancellationToken);

            var places = await _placeService.ListAsync();

            await _bot.EditMessageTextAsync(new(callbackQuery.From.Id),
                callbackQuery.Message.MessageId,
                _localizer[nameof(Messages.SelectPlace)],
                replyMarkup: ReplyMarkupHelpers.GetPlacesMarkup(item.Id, places), cancellationToken: cancellationToken);
        }
    }
}