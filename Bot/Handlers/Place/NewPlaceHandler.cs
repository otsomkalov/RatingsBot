using Bot.Commands.Place;
using Bot.Resources;
using Microsoft.Extensions.Localization;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Handlers.Place;

public class NewPlaceHandler : AsyncRequestHandler<NewPlace>
{
    private readonly ITelegramBotClient _bot;
    private readonly IStringLocalizer<Messages> _localizer;

    public NewPlaceHandler(ITelegramBotClient bot, IStringLocalizer<Messages> localizer)
    {
        _bot = bot;
        _localizer = localizer;
    }

    protected override async Task Handle(NewPlace request, CancellationToken cancellationToken)
    {
        var message = request.Message;

        await _bot.SendTextMessageAsync(new(message.From.Id),
            _localizer[nameof(Messages.NewPlaceCommand)],
            replyMarkup: new ForceReplyMarkup(),
            replyToMessageId: message.MessageId,
            cancellationToken: cancellationToken);
    }
}