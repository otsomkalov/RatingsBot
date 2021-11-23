using Microsoft.Extensions.Localization;
using RatingsBot.Commands.Message;
using RatingsBot.Resources;
using Telegram.Bot.Types.ReplyMarkups;

namespace RatingsBot.Handlers.Message;

public class NewPlaceCommandHandler : AsyncRequestHandler<NewPlaceCommand>
{
    private readonly ITelegramBotClient _bot;
    private readonly IStringLocalizer<Messages> _localizer;

    public NewPlaceCommandHandler(ITelegramBotClient bot, IStringLocalizer<Messages> localizer)
    {
        _bot = bot;
        _localizer = localizer;
    }

    protected override async Task Handle(NewPlaceCommand request, CancellationToken cancellationToken)
    {
        var message = request.Message;

        await _bot.SendTextMessageAsync(new(message.From.Id),
            _localizer[Messages.NewPlaceCommand],
            replyMarkup: new ForceReplyMarkup(),
            replyToMessageId: request.Message.MessageId,
            cancellationToken: cancellationToken);
    }
}