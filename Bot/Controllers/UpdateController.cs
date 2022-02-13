using Bot.Commands.CallbackQuery;
using Bot.Commands.InlineQuery;
using Bot.Commands.Message;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Controllers;

[ApiController]
[Route("update")]
public class UpdateController : ControllerBase
{
    private readonly ILogger<UpdateController> _logger;
    private readonly IMediator _mediator;

    public UpdateController(ILogger<UpdateController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpPost]
    public async Task HandleUpdateAsync(Update update)
    {
        IRequest command = update.Type switch
        {
            UpdateType.Message => new ProcessNewMessage(update.Message),
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