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

public class NewPlaceCommandHandler : AsyncRequestHandler<NewPlaceCommand>
{
    private readonly ITelegramBotClient _bot;
    private readonly IStringLocalizer<Messages> _localizer;
    private readonly PlaceService _placeService;

    public NewPlaceCommandHandler(ITelegramBotClient bot, IStringLocalizer<Messages> localizer, PlaceService placeService)
    {
        _bot = bot;
        _localizer = localizer;
        _placeService = placeService;
    }

    protected override async Task Handle(NewPlaceCommand request, CancellationToken cancellationToken)
    {
        var message = request.Message;

        if (message.Text.Length == Constants.Commands.NewPlace.Length)
        {
            await _bot.SendTextMessageAsync(new(message.From.Id),
                _localizer[ResourcesNames.NewPlaceCommand], cancellationToken: cancellationToken);

            return;
        }

        await _placeService.AddAsync(message.Text[Constants.Commands.NewPlace.Length..].Trim());

        await _bot.SendTextMessageAsync(new(message.From.Id),
            _localizer[ResourcesNames.Created], cancellationToken: cancellationToken);
    }
}