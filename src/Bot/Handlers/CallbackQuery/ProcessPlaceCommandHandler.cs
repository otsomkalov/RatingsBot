using Bot.Constants;
using Bot.Requests.CallbackQuery;
using Bot.Requests.InlineKeyboardMarkup;
using Bot.Requests.Message;
using Bot.Requests.Message.Item;
using Core.Requests.Item;

namespace Bot.Handlers.CallbackQuery;

public class ProcessPlaceCommandHandler : IRequestHandler<ProcessPlaceCommand, Unit>
{
    private readonly ITelegramBotClient _bot;
    private readonly IMediator _mediator;

    public ProcessPlaceCommandHandler(IMediator mediator, ITelegramBotClient bot)
    {
        _mediator = mediator;
        _bot = bot;
    }

    public async Task<Unit> Handle(ProcessPlaceCommand request, CancellationToken cancellationToken)
    {
        var callbackQueryData = request.CallbackQueryData;

        if (callbackQueryData.EntityId is 0 or -1)
        {
            var placesMarkup = await _mediator.Send(
                new GetInlineKeyboardMarkup(callbackQueryData.ItemId, ReplyMarkup.Place, callbackQueryData.Page),
                cancellationToken);

            await _mediator.Send(new EditMessageReplyMarkup(callbackQueryData, placesMarkup), cancellationToken);

            return Unit.Value;
        }

        await _mediator.Send(new SetItemPlace(callbackQueryData.ItemId, callbackQueryData.EntityId), cancellationToken);

        var item = await _mediator.Send(new GetItem(callbackQueryData.ItemId), cancellationToken);
        var ratingsMarkup = await _mediator.Send(new GetRatingsMarkup(callbackQueryData.ItemId), cancellationToken);
        var messageText = await _mediator.Send(new GetItemMessageText(item), cancellationToken);

        await _bot.EditMessageTextAsync(new(callbackQueryData.UserId),
            callbackQueryData.MessageId.Value,
            messageText,
            replyMarkup: ratingsMarkup,
            cancellationToken: cancellationToken);

        return Unit.Value;
    }
}