using Microsoft.Extensions.Localization;
using RatingsBot.Commands.Message;
using RatingsBot.Resources;
using Telegram.Bot.Types.ReplyMarkups;

namespace RatingsBot.Handlers.Message;

public class NewItemCommandHandler : AsyncRequestHandler<NewItemCommand>
{
    private readonly ITelegramBotClient _bot;
    private readonly IStringLocalizer<Messages> _localizer;

    public NewItemCommandHandler(ITelegramBotClient bot, IStringLocalizer<Messages> localizer)
    {
        _bot = bot;
        _localizer = localizer;
    }

    protected override async Task Handle(NewItemCommand request, CancellationToken cancellationToken)
    {
        var message = request.Message;

        await _bot.SendTextMessageAsync(new(message.From.Id),
            _localizer[Messages.NewItemCommand],
            replyMarkup: new ForceReplyMarkup(),
            replyToMessageId: request.Message.MessageId,
            cancellationToken: cancellationToken);
    }
}