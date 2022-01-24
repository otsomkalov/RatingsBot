using Bot.Commands.CallbackQuery;
using Bot.Resources;
using Microsoft.Extensions.Localization;

namespace Bot.Handlers.CallbackQuery;

public class SetPlaceCommandHandler : AsyncRequestHandler<SetPlaceCommand>
{
    private readonly PlaceService _placeService;
    private readonly ITelegramBotClient _bot;
    private readonly IStringLocalizer<Messages> _localizer;
    private readonly AppDbContext _context;

    public SetPlaceCommandHandler(PlaceService placeService, ITelegramBotClient bot,
        IStringLocalizer<Messages> localizer, AppDbContext context)
    {
        _placeService = placeService;
        _bot = bot;
        _localizer = localizer;
        _context = context;
    }

    protected override async Task Handle(SetPlaceCommand request, CancellationToken cancellationToken)
    {
        var (callbackQuery, entityId, item) = request;

        if (entityId is -1)
        {
            var places = await _placeService.ListAsync();

            await _bot.EditMessageReplyMarkupAsync(new(callbackQuery.From.Id),
                callbackQuery.Message.MessageId,
                ReplyMarkupHelpers.GetPlacesMarkup(item.Id, places), cancellationToken);
        }
        else
        {
            item.PlaceId = entityId;

            _context.Update(item);
            await _context.SaveChangesAsync(cancellationToken);

            await _bot.EditMessageTextAsync(new(callbackQuery.From.Id),
                callbackQuery.Message.MessageId,
                MessageHelpers.GetItemMessageText(item, _localizer[nameof(Messages.ItemMessageTemplate)],
                    _localizer[nameof(Messages.RatingLineTemplate)]),
                replyMarkup: ReplyMarkupHelpers.GetRatingsMarkup(item.Id),
                cancellationToken: cancellationToken);
        }
    }
}