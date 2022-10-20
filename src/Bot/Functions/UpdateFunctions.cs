using Bot.Requests.CallbackQuery;
using Bot.Requests.InlineQuery;
using Bot.Requests.Message;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Functions;

public class UpdateFunctions
{
    private readonly ILogger<UpdateFunctions> _logger;
    private readonly IMediator _mediator;

    public UpdateFunctions(ILogger<UpdateFunctions> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [FunctionName(nameof(HandleUpdateAsync))]
    public async Task HandleUpdateAsync([HttpTrigger(AuthorizationLevel.Function, "POST", Route = "update")]Update update)
    {
        IRequest<Unit> command = update.Type switch
        {
            UpdateType.Message => new ProcessMessage(update.Message),
            UpdateType.CallbackQuery => new ProcessCallbackQuery(update.CallbackQuery),
            UpdateType.InlineQuery => new ProcessInlineQuery(update.InlineQuery),
            _ => null
        };

        if (command == null)
        {
            return;
        }

        try
        {
            await _mediator.Send(command);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during processing update:");
        }
    }
}