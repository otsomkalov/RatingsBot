using System.Collections.Immutable;
using Bot.Commands.CallbackQuery;
using Bot.Commands.Item;
using Bot.Commands.User;
using Bot.Constants;

namespace Bot.Handlers.CallbackQuery;

public class ProcessCallbackQueryHandler : AsyncRequestHandler<ProcessCallbackQuery>
{
    private readonly IMediator _mediator;

    public ProcessCallbackQueryHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    protected override async Task Handle(ProcessCallbackQuery request, CancellationToken cancellationToken)
    {
        var callbackQuery = request.CallbackQuery;

        await _mediator.Send(new CreateUserIfNotExists(callbackQuery.From.Id), cancellationToken);

        var callbackData = callbackQuery.Data.Split(ReplyMarkup.Separator).ToImmutableList();

        var itemId = int.Parse(callbackData[0]);
        int? entityId = int.TryParse(callbackData[2], out var id) ? id : null;

        IRequest command = callbackData[1] switch
        {
            ReplyMarkup.Category => new SetItemCategory(callbackQuery, entityId, itemId),
            ReplyMarkup.Place => new SetItemPlace(callbackQuery, entityId, itemId),
            ReplyMarkup.Rating => new SetItemRating(callbackQuery, entityId, itemId)
        };

        await _mediator.Send(command, cancellationToken);
    }
}