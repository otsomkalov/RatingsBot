using Bot.Commands.CallbackQuery;
using Bot.Commands.Category;
using Bot.Commands.Item;
using Bot.Commands.Manufacturer;
using Bot.Commands.Place;
using Bot.Commands.Rating;
using Bot.Constants;
using Bot.Models;
using Bot.Resources;
using Core.Commands.Item;
using Core.Commands.User;
using Microsoft.Extensions.Localization;
using Telegram.Bot.Exceptions;

namespace Bot.Handlers.CallbackQuery;

public class ProcessCallbackQueryHandler : IRequestHandler<ProcessCallbackQuery, Unit>
{
    private readonly ITelegramBotClient _bot;
    private readonly IStringLocalizer<Messages> _localizer;
    private readonly IMediator _mediator;

    public ProcessCallbackQueryHandler(IMediator mediator, ITelegramBotClient bot, IStringLocalizer<Messages> localizer)
    {
        _mediator = mediator;
        _bot = bot;
        _localizer = localizer;
    }

    public async Task<Unit> Handle(ProcessCallbackQuery request, CancellationToken cancellationToken)
    {
        var callbackQuery = request.CallbackQuery;

        await _mediator.Send(new CreateUserIfNotExists(callbackQuery.From.Id, callbackQuery.From.FirstName), cancellationToken);

        var callbackQueryData = new CallbackQueryData(callbackQuery);

        var commandToExecute = callbackQueryData.Command switch
        {
            ReplyMarkup.Category => ProcessCategoryCommand(callbackQueryData, cancellationToken),
            ReplyMarkup.Place => ProcessPlaceCommand(callbackQueryData, cancellationToken),
            ReplyMarkup.Manufacturer => ProcessManufacturerCommand(callbackQueryData, cancellationToken),
            ReplyMarkup.Rating => ProcessRatingCommand(callbackQueryData, cancellationToken)
        };

        await commandToExecute;

        return Unit.Value;
    }

    private async Task ProcessRatingCommand(CallbackQueryData callbackQueryData, CancellationToken cancellationToken)
    {
        if (!callbackQueryData.EntityId.HasValue)
        {
            var item = await _mediator.Send(new GetItem(callbackQueryData.ItemId), cancellationToken);

            var messageText = await _mediator.Send(new GetItemMessageText(item), cancellationToken);

            try
            {
                var ratingsMarkup = await _mediator.Send(new GetRatingsMarkup(item.Id), cancellationToken);

                if (callbackQueryData.InlineMessageId != null)
                {
                    await _bot.EditMessageTextAsync(callbackQueryData.InlineMessageId,
                        messageText,
                        replyMarkup: ratingsMarkup,
                        cancellationToken: cancellationToken);
                }
                else
                {
                    await _bot.EditMessageTextAsync(new(callbackQueryData.UserId),
                        callbackQueryData.MessageId,
                        messageText,
                        replyMarkup: ratingsMarkup,
                        cancellationToken: cancellationToken);
                }
            }
            catch (ApiRequestException)
            {
                await _bot.AnswerCallbackQueryAsync(callbackQueryData.QueryId,
                    _localizer[nameof(Messages.Refreshed)], cancellationToken: cancellationToken);
            }

            return;
        }

        var command = new SetItemRating(callbackQueryData.UserId, callbackQueryData.EntityId, callbackQueryData.ItemId);

        await _mediator.Send(command, cancellationToken);

        await _bot.AnswerCallbackQueryAsync(callbackQueryData.QueryId, _localizer[nameof(Messages.Recorded)],
            cancellationToken: cancellationToken);
    }

    private async Task ProcessManufacturerCommand(CallbackQueryData callbackQueryData, CancellationToken cancellationToken)
    {
        if (callbackQueryData.EntityId is -1)
        {
            var manufacturersMarkup = await _mediator.Send(new GetManufacturersMarkup(callbackQueryData.ItemId), cancellationToken);

            await _bot.EditMessageReplyMarkupAsync(new(callbackQueryData.UserId),
                callbackQueryData.MessageId,
                manufacturersMarkup,
                cancellationToken);

            return;
        }

        var command = new SetItemManufacturer(callbackQueryData.EntityId, callbackQueryData.ItemId);
        await _mediator.Send(command, cancellationToken);
        var placesMarkup = await _mediator.Send(new GetPlacesMarkup(callbackQueryData.ItemId), cancellationToken);

        await _bot.EditMessageTextAsync(new(callbackQueryData.UserId),
            callbackQueryData.MessageId,
            _localizer[nameof(Messages.SelectPlace)],
            replyMarkup: placesMarkup,
            cancellationToken: cancellationToken);
    }

    private async Task ProcessCategoryCommand(CallbackQueryData callbackQueryData, CancellationToken cancellationToken)
    {
        if (!callbackQueryData.EntityId.HasValue)
        {
            var categoriesMarkup = await _mediator.Send(new GetCategoriesMarkup(callbackQueryData.ItemId), cancellationToken);

            await _bot.EditMessageReplyMarkupAsync(new(callbackQueryData.UserId),
                callbackQueryData.MessageId,
                categoriesMarkup,
                cancellationToken);

            return;
        }

        await _mediator.Send(new SetItemCategory(callbackQueryData.EntityId.Value, callbackQueryData.ItemId), cancellationToken);

        var manufacturersMarkup = await _mediator.Send(new GetManufacturersMarkup(callbackQueryData.ItemId), cancellationToken);

        await _bot.EditMessageTextAsync(new(callbackQueryData.UserId),
            callbackQueryData.MessageId,
            _localizer[nameof(Messages.SelectManufacturer)],
            replyMarkup: manufacturersMarkup,
            cancellationToken: cancellationToken);
    }

    private async Task ProcessPlaceCommand(CallbackQueryData callbackQueryData, CancellationToken cancellationToken)
    {
        if (callbackQueryData.EntityId is -1)
        {
            var placesMarkup = await _mediator.Send(new GetPlacesMarkup(callbackQueryData.ItemId), cancellationToken);

            await _bot.EditMessageReplyMarkupAsync(new(callbackQueryData.UserId),
                callbackQueryData.MessageId,
                placesMarkup, cancellationToken);

            return;
        }

        await _mediator.Send(new SetItemPlace(callbackQueryData.EntityId, callbackQueryData.ItemId), cancellationToken);

        var item = await _mediator.Send(new GetItem(callbackQueryData.ItemId), cancellationToken);
        var ratingsMarkup = await _mediator.Send(new GetRatingsMarkup(callbackQueryData.ItemId), cancellationToken);
        var messageText = await _mediator.Send(new GetItemMessageText(item), cancellationToken);

        await _bot.EditMessageTextAsync(new(callbackQueryData.UserId),
            callbackQueryData.MessageId,
            messageText,
            replyMarkup: ratingsMarkup,
            cancellationToken: cancellationToken);
    }
}