using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Localization;
using RatingsBot.Commands.CallbackQuery;
using RatingsBot.Constants;
using RatingsBot.Helpers;
using RatingsBot.Resources;
using RatingsBot.Services;
using Telegram.Bot;

namespace RatingsBot.Handlers.CallbackQuery;

public class SetPlaceCommandHandler : AsyncRequestHandler<SetPlaceCommand>
{
    private readonly PlaceService _placeService;
    private readonly ITelegramBotClient _bot;
    private readonly ItemService _itemService;
    private readonly IStringLocalizer<Messages> _localizer;

    public SetPlaceCommandHandler(PlaceService placeService, ITelegramBotClient bot, ItemService itemService,
        IStringLocalizer<Messages> localizer)
    {
        _placeService = placeService;
        _bot = bot;
        _itemService = itemService;
        _localizer = localizer;
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
            await _itemService.UpdatePlaceAsync(item, entityId);

            await _bot.EditMessageTextAsync(new(callbackQuery.From.Id),
                callbackQuery.Message.MessageId,
                MessageHelpers.GetItemMessageText(item, callbackQuery.From.Id, _localizer[ResourcesNames.ItemMessageTemplate]),
                replyMarkup: ReplyMarkupHelpers.GetRatingsMarkup(item.Id), cancellationToken: cancellationToken);
        }
    }
}