using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Localization;
using RatingsBot.Commands.Message;
using RatingsBot.Constants;
using RatingsBot.Resources;
using RatingsBot.Services;
using Telegram.Bot;

namespace RatingsBot.Handlers.Message;

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
            _localizer[ResourcesNames.Welcome], cancellationToken: cancellationToken);
    }
}