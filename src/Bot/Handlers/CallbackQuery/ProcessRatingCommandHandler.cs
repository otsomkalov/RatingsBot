using Bot.Requests.CallbackQuery;
using Bot.Requests.InlineKeyboardMarkup;
using Bot.Requests.Message.Item;
using Bot.Resources;
using Core.Requests.Item;
using Core.Requests.Rating;
using Microsoft.Extensions.Localization;
using Telegram.Bot.Exceptions;

namespace Bot.Handlers.CallbackQuery;

public class ProcessRatingCommandHandler : IRequestHandler<ProcessRatingCommand, Unit>
{
    private readonly ITelegramBotClient _bot;
    private readonly IStringLocalizer<Messages> _localizer;
    private readonly IMediator _mediator;

    public ProcessRatingCommandHandler(IMediator mediator, ITelegramBotClient bot, IStringLocalizer<Messages> localizer)
    {
        _mediator = mediator;
        _bot = bot;
        _localizer = localizer;
    }

    public async Task<Unit> Handle(ProcessRatingCommand request, CancellationToken cancellationToken)
    {
        var callbackQueryData = request.CallbackQueryData;

        if (callbackQueryData.RatingValue != 0)
        {
            var existingRating = await _mediator.Send(new GetRating(callbackQueryData.ItemId, callbackQueryData.UserId), cancellationToken);

            if (existingRating == null || callbackQueryData.RatingValue != existingRating.Value)
            {
                await _mediator.Send(new SetItemRating(callbackQueryData.ItemId, callbackQueryData.UserId, callbackQueryData.RatingValue),
                    cancellationToken);
            }

            await _bot.AnswerCallbackQueryAsync(callbackQueryData.QueryId, _localizer[nameof(Messages.Recorded)],
                cancellationToken: cancellationToken);
        }

        var item = await _mediator.Send(new GetItem(callbackQueryData.ItemId), cancellationToken);
        var messageText = await _mediator.Send(new GetItemMessageText(item), cancellationToken);
        var ratingsMarkup = await _mediator.Send(new GetRatingsMarkup(item.Id), cancellationToken);

        try
        {
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
                    callbackQueryData.MessageId.Value,
                    messageText,
                    replyMarkup: ratingsMarkup,
                    cancellationToken: cancellationToken);
            }
        }
        catch (ApiRequestException)
        {
            await _bot.AnswerCallbackQueryAsync(callbackQueryData.QueryId, _localizer[nameof(Messages.Refreshed)],
                cancellationToken: cancellationToken);
        }

        return Unit.Value;
    }
}