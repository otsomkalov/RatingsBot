using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Controllers;

[ApiController]
[Route("update")]
public class UpdateController : ControllerBase
{
    private readonly MessageService _messageService;
    private readonly CallbackQueryService _callbackQueryService;
    private readonly InlineQueryService _inlineQueryService;
    private readonly ILogger<UpdateController> _logger;

    public UpdateController(MessageService messageService, CallbackQueryService callbackQueryService, InlineQueryService inlineQueryService,
        ILogger<UpdateController> logger)
    {
        _messageService = messageService;
        _callbackQueryService = callbackQueryService;
        _inlineQueryService = inlineQueryService;
        _logger = logger;
    }

    [HttpPost]
    public async Task HandleUpdateAsync(Update update)
    {
        var handleUpdateTask = update.Type switch
        {
            UpdateType.Message => _messageService.HandleAsync(update.Message),
            UpdateType.CallbackQuery => _callbackQueryService.HandleAsync(update.CallbackQuery),
            UpdateType.InlineQuery => _inlineQueryService.HandleAsync(update.InlineQuery),
            _ => Task.CompletedTask
        };

        try
        {
            await handleUpdateTask;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during processing update:");
        }
    }
}