using Bot.Constants;
using Bot.Models;
using Bot.Requests.CallbackQuery;
using Core.Requests.User;

namespace Bot.Handlers.CallbackQuery;

public class ProcessCallbackQueryHandler : IRequestHandler<ProcessCallbackQuery, Unit>
{
    private readonly IMediator _mediator;

    public ProcessCallbackQueryHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<Unit> Handle(ProcessCallbackQuery request, CancellationToken cancellationToken)
    {
        var callbackQuery = request.CallbackQuery;

        await _mediator.Send(new CreateUserIfNotExists(callbackQuery.From.Id, callbackQuery.From.FirstName), cancellationToken);

        IRequest commandToExecute = null;

        if (callbackQuery.Data.Contains(ReplyMarkup.Category))
        {
            var callbackQueryData = new EntitiesCallbackQueryData(callbackQuery);
            commandToExecute = new ProcessCategoryCommand(callbackQueryData);
        }
        else if (callbackQuery.Data.Contains(ReplyMarkup.Place))
        {
            var callbackQueryData = new EntitiesCallbackQueryData(callbackQuery);
            commandToExecute = new ProcessPlaceCommand(callbackQueryData);
        }
        else if (callbackQuery.Data.Contains(ReplyMarkup.Manufacturer))
        {
            var callbackQueryData = new EntitiesCallbackQueryData(callbackQuery);
            commandToExecute = new ProcessManufacturerCommand(callbackQueryData);
        }
        else if (callbackQuery.Data.Contains(ReplyMarkup.Rating))
        {
            var callbackQueryData = new RatingCallbackQueryData(callbackQuery);
            commandToExecute = new ProcessRatingCommand(callbackQueryData);
        }

        await _mediator.Send(commandToExecute, cancellationToken);

        return Unit.Value;
    }
}