using Bot.Requests.CallbackQuery;
using Bot.Requests.Item;
using Bot.Requests.Rating;
using Bot.Resources;
using Core.Requests.Item;
using Microsoft.Extensions.Localization;

namespace Bot.Handlers.CallbackQuery;

public class ProcessRatingCommandHandler : IRequestHandler<ProcessRatingCommand, Unit>
{
    private readonly IMediator _mediator;
    private readonly ITelegramBotClient _bot;
    private readonly IStringLocalizer<Messages> _localizer;

    public ProcessRatingCommandHandler(IMediator mediator, ITelegramBotClient bot, IStringLocalizer<Messages> localizer)
    {
        _mediator = mediator;
        _bot = bot;
        _localizer = localizer;
    }

    public async Task<Unit> Handle(ProcessRatingCommand request, CancellationToken cancellationToken)
    {
        var callbackQueryData = request.CallbackQueryData;

        if (callbackQueryData.EntityId is not null and not 0)
        {
            var command = new SetItemRating(callbackQueryData.UserId, callbackQueryData.EntityId.Value, callbackQueryData.ItemId);

            await _mediator.Send(command, cancellationToken);

            await _bot.AnswerCallbackQueryAsync(callbackQueryData.QueryId, _localizer[nameof(Messages.Recorded)],
                cancellationToken: cancellationToken);
        }

        var item = await _mediator.Send(new GetItem(callbackQueryData.ItemId), cancellationToken);

        var currentUserRating = item.Ratings.FirstOrDefault(r => r.UserId == callbackQueryData.UserId);

        if (currentUserRating?.Value == callbackQueryData.EntityId)
        {
            await _bot.AnswerCallbackQueryAsync(callbackQueryData.QueryId,
                _localizer[nameof(Messages.Recorded)],
                cancellationToken: cancellationToken);

            return Unit.Value;
        }

        var messageText = await _mediator.Send(new GetItemMessageText(item), cancellationToken);
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
                callbackQueryData.MessageId.Value,
                messageText,
                replyMarkup: ratingsMarkup,
                cancellationToken: cancellationToken);
        }

        return Unit.Value;
    }
}