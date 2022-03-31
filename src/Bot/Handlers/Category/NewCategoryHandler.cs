using Bot.Commands.Category;
using Bot.Resources;
using Microsoft.Extensions.Localization;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Handlers.Category;

public class NewCategoryHandler : AsyncRequestHandler<NewCategory>
{
    private readonly ITelegramBotClient _bot;
    private readonly IStringLocalizer<Messages> _localizer;

    public NewCategoryHandler(ITelegramBotClient bot, IStringLocalizer<Messages> localizer)
    {
        _bot = bot;
        _localizer = localizer;
    }

    protected override async Task Handle(NewCategory request, CancellationToken cancellationToken)
    {
        var message = request.Message;

        await _bot.SendTextMessageAsync(new(message.From.Id),
            _localizer[nameof(Messages.NewCategoryCommand)],
            replyMarkup: new ForceReplyMarkup(),
            replyToMessageId: request.Message.MessageId,
            cancellationToken: cancellationToken);
    }
}