using Bot.Commands.CallbackQuery;
using Bot.Resources;
using Microsoft.Extensions.Localization;
using Telegram.Bot.Exceptions;

namespace Bot.Handlers.CallbackQuery;

public class SetRatingCommandHandler : AsyncRequestHandler<SetRatingCommand>
{
    private readonly RatingService _ratingService;
    private readonly ITelegramBotClient _bot;
    private readonly IStringLocalizer<Messages> _localizer;

    public SetRatingCommandHandler(RatingService ratingService, ITelegramBotClient bot, IStringLocalizer<Messages> localizer)
    {
        _ratingService = ratingService;
        _bot = bot;
        _localizer = localizer;
    }

    protected override async Task Handle(SetRatingCommand request, CancellationToken cancellationToken)
    {
        var (callbackQuery, entityId, item) = request;

        if (entityId.HasValue)
        {
            await _ratingService.UpsertAsync(callbackQuery.From.Id, item.Id, entityId.Value);

            await _bot.AnswerCallbackQueryAsync(callbackQuery.Id, _localizer[nameof(Messages.Recorded)], cancellationToken: cancellationToken);
        }

        var messageText = MessageHelpers.GetItemMessageText(item, _localizer[nameof(Messages.ItemMessageTemplate)],
            _localizer[nameof(Messages.RatingLineTemplate)]);

        try
        {
            if (callbackQuery.InlineMessageId != null)
            {
                await _bot.EditMessageTextAsync(callbackQuery.InlineMessageId,
                    messageText,
                    replyMarkup: ReplyMarkupHelpers.GetRatingsMarkup(item.Id), cancellationToken: cancellationToken);
            }
            else
            {
                await _bot.EditMessageTextAsync(new(callbackQuery.From.Id),
                    callbackQuery.Message.MessageId,
                    messageText,
                    replyMarkup: ReplyMarkupHelpers.GetRatingsMarkup(item.Id), cancellationToken: cancellationToken);
            }
        }
        catch (MessageIsNotModifiedException)
        {
            await _bot.AnswerCallbackQueryAsync(callbackQuery.Id,
                _localizer[nameof(Messages.Refreshed)], cancellationToken: cancellationToken);
        }
    }
}