using Bot.Commands.Message;
using Bot.Resources;
using Microsoft.Extensions.Localization;
using Telegram.Bot.Types;

namespace Bot.Services;

public class MessageService
{
    private readonly IStringLocalizer<Messages> _localizer;
    private readonly IMediator _mediator;

    public MessageService(IMediator mediator, IStringLocalizer<Messages> localizer)
    {
        _mediator = mediator;
        _localizer = localizer;
    }

    public async Task HandleAsync(Message message)
    {
        if (message.From.IsBot)
        {
            return;
        }

        MessageCommand request = message.Text.Trim() switch
        {
            Constants.Commands.Start => new StartCommand(message),
            Constants.Commands.NewPlace => new NewPlaceCommand(message),
            Constants.Commands.NewCategory => new NewCategoryCommand(message),
            Constants.Commands.NewItem => new NewItemCommand(message),
            _ => null
        };

        if (message.ReplyToMessage?.Text == _localizer[Messages.NewCategoryCommand])
        {
            request = new CreateCategoryCommand(message);
        }
        else if (message.ReplyToMessage?.Text == _localizer[Messages.NewPlaceCommand])
        {
            request = new CreatePlaceCommand(message);
        }
        else if (message.ReplyToMessage?.Text == _localizer[Messages.NewItemCommand])
        {
            request = new CreateItemCommand(message);
        }

        if (request != null)
        {
            await _mediator.Send(request);
        }
    }
}