using Bot.Constants;
using Bot.Requests.CallbackQuery;
using Bot.Requests.InlineKeyboardMarkup;
using Bot.Requests.Message;
using Bot.Resources;
using Core.Requests.Item;
using Microsoft.Extensions.Localization;

namespace Bot.Handlers.CallbackQuery;

public class ProcessManufacturerCommandHandler : IRequestHandler<ProcessManufacturerCommand, Unit>
{
    private readonly ITelegramBotClient _bot;
    private readonly IStringLocalizer<Messages> _localizer;
    private readonly IMediator _mediator;

    public ProcessManufacturerCommandHandler(IMediator mediator, ITelegramBotClient bot, IStringLocalizer<Messages> localizer)
    {
        _mediator = mediator;
        _bot = bot;
        _localizer = localizer;
    }

    public async Task<Unit> Handle(ProcessManufacturerCommand request, CancellationToken cancellationToken)
    {
        var callbackQueryData = request.CallbackQueryData;

        if (callbackQueryData.EntityId is 0 or -1)
        {
            var manufacturersMarkup = await _mediator.Send(
                new GetInlineKeyboardMarkup(callbackQueryData.ItemId, ReplyMarkup.Manufacturer, callbackQueryData.Page),
                cancellationToken);

            await _mediator.Send(new EditMessageReplyMarkup(callbackQueryData, manufacturersMarkup), cancellationToken);

            return Unit.Value;
        }

        var command = new SetItemManufacturer(callbackQueryData.ItemId, callbackQueryData.EntityId);
        await _mediator.Send(command, cancellationToken);

        var placesMarkup =
            await _mediator.Send(new GetInlineKeyboardMarkup(callbackQueryData.ItemId, ReplyMarkup.Place), cancellationToken);

        await _bot.EditMessageTextAsync(new(callbackQueryData.UserId),
            callbackQueryData.MessageId.Value,
            _localizer[nameof(Messages.SelectPlace)],
            replyMarkup: placesMarkup,
            cancellationToken: cancellationToken);

        return Unit.Value;
    }
}