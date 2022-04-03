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

        if (callbackQuery.Data.Contains(ReplyMarkup.Rating))
        {
            var ratingCallbackQueryData = new RatingCallbackQueryData(callbackQuery);
            var processRatingCommand = new ProcessRatingCommand(ratingCallbackQueryData);

            await _mediator.Send(processRatingCommand, cancellationToken);

            return Unit.Value;
        }

        if (callbackQuery.Data.Contains(ReplyMarkup.Category))
        {
            var categoryCallbackQueryData = new CategoryCallbackQueryData(callbackQuery);
            var processCategoryCommand = new ProcessCategoryCommand(categoryCallbackQueryData);

            await _mediator.Send(processCategoryCommand, cancellationToken);

            return Unit.Value;
        }

        IRequest commandToExecute = null;

        var entitiesCallbackQueryData = new EntitiesCallbackQueryData(callbackQuery);

        if (callbackQuery.Data.Contains(ReplyMarkup.Place))
        {
            commandToExecute = new ProcessPlaceCommand(entitiesCallbackQueryData);
        }
        else if (callbackQuery.Data.Contains(ReplyMarkup.Manufacturer))
        {
            commandToExecute = new ProcessManufacturerCommand(entitiesCallbackQueryData);
        }

        await _mediator.Send(commandToExecute, cancellationToken);

        return Unit.Value;
    }
}