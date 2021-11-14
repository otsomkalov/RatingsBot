using RatingsBot.Commands.Message;
using Telegram.Bot.Types;

namespace RatingsBot.Services;

public class MessageService
{
    private readonly IMediator _mediator;

    public MessageService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task HandleAsync(Message message)
    {
        await HandleCommandAsync(Constants.Commands.Start, message);
        await HandleCommandAsync(Constants.Commands.NewCategory, message);
        await HandleCommandAsync(Constants.Commands.NewPlace, message);
        await HandleCommandAsync(Constants.Commands.NewItem, message);
    }

    private async Task HandleCommandAsync(string command, Message message)
    {
        if (!message.Text.StartsWithCI(command))
        {
            return;
        }

        MessageCommand request = command switch
        {
            Constants.Commands.Start => new StartCommand(message),
            Constants.Commands.NewCategory => new NewCategoryCommand(message),
            Constants.Commands.NewPlace => new NewPlaceCommand(message),
            Constants.Commands.NewItem => new NewItemCommand(message),
            _ => throw new ArgumentOutOfRangeException(nameof(command), command, null)
        };

        await _mediator.Send(request);
    }
}