using Bot.Commands.Message;
using Bot.Helpers;
using Bot.Resources;
using Bot.Services;
using Microsoft.Extensions.Localization;

namespace Bot.Handlers.Message;

public class StartCommandHandler : AsyncRequestHandler<StartCommand>
{
    private readonly UserService _userService;
    private readonly ITelegramBotClient _bot;
    private readonly IStringLocalizer<Messages> _localizer;

    public StartCommandHandler(UserService userService, ITelegramBotClient bot, IStringLocalizer<Messages> localizer)
    {
        _userService = userService;
        _bot = bot;
        _localizer = localizer;
    }

    protected override async Task Handle(StartCommand request, CancellationToken cancellationToken)
    {
        var message = request.Message;

        await _userService.CreateIfNotExistsAsync(message.From.Id);

        await _bot.SendTextMessageAsync(new(message.From.Id),
            _localizer[nameof(Messages.Welcome)],
            replyMarkup: ReplyKeyboardMarkupHelpers.GetStartReplyKeyboardMarkup(),
            cancellationToken: cancellationToken);
    }
}