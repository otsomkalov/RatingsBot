using Bot.Commands.Message;
using Bot.Helpers;
using Bot.Resources;
using Bot.Services;
using Microsoft.Extensions.Localization;

namespace Bot.Handlers.Message;

public class CreatePlaceCommandHandler : AsyncRequestHandler<CreatePlaceCommand>
{
    private readonly ITelegramBotClient _bot;
    private readonly IStringLocalizer<Messages> _localizer;
    private readonly PlaceService _placeService;

    public CreatePlaceCommandHandler(PlaceService placeService, ITelegramBotClient bot, IStringLocalizer<Messages> localizer)
    {
        _placeService = placeService;
        _bot = bot;
        _localizer = localizer;
    }

    protected override async Task Handle(CreatePlaceCommand request, CancellationToken cancellationToken)
    {
        var message = request.Message;

        await _placeService.AddAsync(message.Text.Trim());

        await _bot.SendTextMessageAsync(new(message.From.Id),
            _localizer[nameof(Messages.Created)],
            replyToMessageId: message.MessageId,
            replyMarkup: ReplyKeyboardMarkupHelpers.GetStartReplyKeyboardMarkup(),
            cancellationToken: cancellationToken);
    }
}