using System.Collections.Immutable;
using Bot.Commands.CallbackQuery;
using Bot.Constants;
using Telegram.Bot.Types;

namespace Bot.Services;

public class CallbackQueryService
{
    private readonly ItemService _itemService;
    private readonly IMediator _mediator;
    private readonly UserService _userService;

    public CallbackQueryService(ItemService itemService, IMediator mediator, UserService userService)
    {
        _itemService = itemService;
        _mediator = mediator;
        _userService = userService;
    }

    public async Task HandleAsync(CallbackQuery callbackQuery)
    {
        await _userService.CreateIfNotExistsAsync(callbackQuery.From.Id);

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