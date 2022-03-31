using Bot.Commands.Manufacturer;
using Bot.Resources;
using Microsoft.Extensions.Localization;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Handlers.Manufacturer;

public class NewManufacturerHandler : AsyncRequestHandler<NewManufacturer>
{
    private readonly ITelegramBotClient _bot;
    private readonly IStringLocalizer<Messages> _localizer;

    public NewManufacturerHandler(ITelegramBotClient bot, IStringLocalizer<Messages> localizer)
    {
        _bot = bot;
        _localizer = localizer;
    }

    protected override async Task Handle(NewManufacturer request, CancellationToken cancellationToken)
    {
        var message = request.Message;

        await _bot.SendTextMessageAsync(new(message.From.Id),
            _localizer[nameof(Messages.NewManufacturerCommand)],
            replyMarkup: new ForceReplyMarkup(),
            replyToMessageId: request.Message.MessageId,
            cancellationToken: cancellationToken);
    }
}