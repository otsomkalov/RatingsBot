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

    public UpdateController(MessageService messageService, CallbackQueryService callbackQueryService, InlineQueryService inlineQueryService)
    {
        _messageService = messageService;
        _callbackQueryService = callbackQueryService;
        _inlineQueryService = inlineQueryService;
    }

    [HttpPost]
    public Task HandleUpdateAsync(Update update)
    {
        return update.Type switch
        {
            UpdateType.Message => _messageService.HandleAsync(update.Message),
            UpdateType.CallbackQuery => _callbackQueryService.HandleAsync(update.CallbackQuery),
            UpdateType.InlineQuery => _inlineQueryService.HandleAsync(update.InlineQuery),
            _ => Task.CompletedTask
        };
    }
}