using Bot.Commands.Category;
using Bot.Commands.Item;
using Bot.Commands.Manufacturer;
using Bot.Commands.Message;
using Bot.Commands.Place;
using Bot.Resources;
using Core.Commands.Category;
using Core.Commands.Item;
using Core.Commands.Manufacturer;
using Core.Commands.Place;
using Core.Commands.User;
using FluentResults;
using Microsoft.Extensions.Localization;

namespace Bot.Handlers.Message;

public class ProcessNewMessageHandler : IRequestHandler<ProcessNewMessage, Unit>
{
    private readonly ITelegramBotClient _bot;
    private readonly IStringLocalizer<Messages> _localizer;
    private readonly IMediator _mediator;

    public ProcessNewMessageHandler(IStringLocalizer<Messages> localizer, IMediator mediator, ITelegramBotClient bot)
    {
        _localizer = localizer;
        _mediator = mediator;
        _bot = bot;
    }

    public async Task<Unit> Handle(ProcessNewMessage request, CancellationToken cancellationToken)
    {
        var message = request.Message;

        if (message.From.IsBot || string.IsNullOrEmpty(message.Text))
        {
            return Unit.Value;
        }

        await _mediator.Send(new CreateUserIfNotExists(message.From.Id, message.From.Username), cancellationToken);

        IRequest messageTextCommand = message.Text.Trim() switch
        {
            Constants.Commands.Start => new ProcessStartCommand(message),
            Constants.Commands.NewPlace => new NewPlace(message),
            Constants.Commands.NewCategory => new NewCategory(message),
            Constants.Commands.NewItem => new NewItem(message),
            Constants.Commands.NewManufacturer => new NewManufacturer(message),
            _ => null
        };

        if (messageTextCommand != null)
        {
            await _mediator.Send(messageTextCommand, cancellationToken);

            return Unit.Value;
        }

        if (message.ReplyToMessage?.Text == _localizer[nameof(Messages.NewItemCommand)])
        {
            var createItemCommand = new CreateItem(message.Text.Trim());
            var createItemResult = await _mediator.Send(createItemCommand, cancellationToken);

            if (createItemResult.IsFailed)
            {
                await _bot.SendTextMessageAsync(new (message.From.Id),
                    string.Join(Environment.NewLine, createItemResult.Errors.Select(e => e.Message)),
                    replyToMessageId: message.MessageId,
                    cancellationToken: cancellationToken);

                return Unit.Value;
            }

            var categoriesMarkup = await _mediator.Send(new GetCategoriesMarkup(createItemResult.Value.Id), cancellationToken);

            await _bot.SendTextMessageAsync(new(message.From.Id),
                _localizer[nameof(Messages.SelectCategory)],
                replyMarkup: categoriesMarkup,
                replyToMessageId: message.MessageId,
                cancellationToken: cancellationToken);

            return Unit.Value;
        }

        IRequest<Result> replyMessageTextCommand = null;

        if (message.ReplyToMessage?.Text == _localizer[nameof(Messages.NewCategoryCommand)])
        {
            replyMessageTextCommand = new CreateCategory(message.Text.Trim());
        }
        else if (message.ReplyToMessage?.Text == _localizer[nameof(Messages.NewPlaceCommand)])
        {
            replyMessageTextCommand = new CreatePlace(message.Text.Trim());
        }
        else if (message.ReplyToMessage?.Text == _localizer[nameof(Messages.NewManufacturerCommand)])
        {
            replyMessageTextCommand = new CreateManufacturer(message.Text.Trim());
        }

        if (replyMessageTextCommand != null)
        {
            await _mediator.Send(replyMessageTextCommand, cancellationToken);
        }

        await _bot.SendTextMessageAsync(new(message.From.Id),
            _localizer[nameof(Messages.Created)],
            replyToMessageId: message.MessageId,
            replyMarkup: ReplyKeyboardMarkupHelpers.GetStartReplyKeyboardMarkup(),
            cancellationToken: cancellationToken);

        return Unit.Value;
    }
}