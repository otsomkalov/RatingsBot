using Bot.Requests.Message;
using Bot.Resources;
using Microsoft.Extensions.Localization;
using Telegram.Bot.Exceptions;

namespace Bot.Handlers.Message;

public class EditMessageReplyMarkupHandler : IRequestHandler<EditMessageReplyMarkup>
{
    private readonly ITelegramBotClient _bot;
    private readonly IStringLocalizer<Messages> _localizer;

    public EditMessageReplyMarkupHandler(ITelegramBotClient bot, IStringLocalizer<Messages> localizer)
    {
        _bot = bot;
        _localizer = localizer;
    }

    public async Task<Unit> Handle(EditMessageReplyMarkup request, CancellationToken cancellationToken)
    {
        var (callbackQueryData, inlineKeyboardMarkup) = request;

        try
        {
            await _bot.EditMessageReplyMarkupAsync(new(callbackQueryData.UserId),
                callbackQueryData.MessageId.Value,
                inlineKeyboardMarkup, cancellationToken);
        }
        catch (ApiRequestException)
        {
            await _bot.AnswerCallbackQueryAsync(callbackQueryData.QueryId,
                _localizer[nameof(Messages.Refreshed)],
                cancellationToken: cancellationToken);
        }

        return Unit.Value;
    }
}