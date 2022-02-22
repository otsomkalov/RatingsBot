using Bot.Commands.Category;
using Bot.Commands.Item;
using Bot.Commands.Manufacturer;
using Bot.Commands.Message;
using Bot.Commands.Place;
using Bot.Commands.User;
using Bot.Resources;
using Microsoft.Extensions.Localization;

namespace Bot.Handlers.Message;

public class ProcessNewMessageHandler : AsyncRequestHandler<ProcessNewMessage>
{
    private readonly IStringLocalizer<Messages> _localizer;
    private readonly IMediator _mediator;

    public ProcessNewMessageHandler(IStringLocalizer<Messages> localizer, IMediator mediator)
    {
        _localizer = localizer;
        _mediator = mediator;
    }

    protected override async Task Handle(ProcessNewMessage request, CancellationToken cancellationToken)
    {
        var message = request.Message;

        if (message.From.IsBot)
        {
            return;
        }

        await _mediator.Send(new CreateUserIfNotExists(message.From.Id), cancellationToken);

        IRequest command = message.Text.Trim() switch
        {
            Constants.Commands.Start => new ProcessStartCommand(message),
            Constants.Commands.NewPlace => new NewPlace(message),
            Constants.Commands.NewCategory => new NewCategory(message),
            Constants.Commands.NewItem => new NewItem(message),
            Constants.Commands.NewManufacturer => new NewManufacturer(message),
            _ => null
        };

        if (command != null)
        {
            await _mediator.Send(command, cancellationToken);

            return;
        }

        if (message.ReplyToMessage?.Text == _localizer[nameof(Messages.NewCategoryCommand)])
        {
            command = new CreateCategory(message);
        }
        else if (message.ReplyToMessage?.Text == _localizer[nameof(Messages.NewPlaceCommand)])
        {
            command = new CreatePlace(message);
        }
        else if (message.ReplyToMessage?.Text == _localizer[nameof(Messages.NewItemCommand)])
        {
            command = new CreateItem(message);
        }
        else if (message.ReplyToMessage?.Text == _localizer[nameof(Messages.NewManufacturerCommand)])
        {
            command = new CreateManufacturer(message);
        }

        if (command != null)
        {
            await _mediator.Send(command, cancellationToken);
        }
    }
}