using Bot.Commands.Item;
using Bot.Resources;
using Microsoft.Extensions.Localization;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Handlers.Item;

public class NewItemHandler : AsyncRequestHandler<NewItem>
{
    private readonly ITelegramBotClient _bot;
    private readonly IStringLocalizer<Messages> _localizer;

    public NewItemHandler(ITelegramBotClient bot, IStringLocalizer<Messages> localizer)
    {
        _bot = bot;
        _localizer = localizer;
    }

    protected override async Task Handle(NewItem request, CancellationToken cancellationToken)
    {
        var message = request.Message;

        await _bot.SendTextMessageAsync(new(message.From.Id),
            _localizer[nameof(Messages.NewItemCommand)],
            replyMarkup: new ForceReplyMarkup(),
            replyToMessageId: request.Message.MessageId,
            cancellationToken: cancellationToken);
    }
}