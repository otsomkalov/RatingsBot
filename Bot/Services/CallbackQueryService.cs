using System.Collections.Immutable;
using RatingsBot.Commands.CallbackQuery;
using RatingsBot.Constants;
using Telegram.Bot.Types;

namespace RatingsBot.Services;

public class CallbackQueryService
{
    private readonly ItemService _itemService;
    private readonly IMediator _mediator;

    public CallbackQueryService(ItemService itemService, IMediator mediator)
    {
        _itemService = itemService;
        _mediator = mediator;
    }

    public async Task HandleAsync(CallbackQuery callbackQuery)
    {
        var callbackData = callbackQuery.Data.Split(ReplyMarkup.Separator).ToImmutableList();

        var itemId = int.Parse(callbackData[0]);
        int? entityId = int.TryParse(callbackData[2], out var id) ? id : null;
        var item = await _itemService.GetAsync(itemId);

        CallbackQueryCommand command = callbackData[1] switch
        {
            ReplyMarkup.Category => new SetCategoryCommand(callbackQuery, entityId, item),
            ReplyMarkup.Place => new SetPlaceCommand(callbackQuery, entityId, item),
            ReplyMarkup.Rating => new SetRatingCommand(callbackQuery, entityId, item)
        };

        await _mediator.Send(command);
    }
}