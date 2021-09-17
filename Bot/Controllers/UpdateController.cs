using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RatingsBot.Services;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace RatingsBot.Controllers
{
    [ApiController]
    [Route("update")]
    public class UpdateController : ControllerBase
    {
        private readonly MessageService _messageService;
        private readonly CallbackQueryService _callbackQueryService;
        private readonly InlineQueryService _inlineQueryService;

        public UpdateController(MessageService messageService, CallbackQueryService callbackQueryService, InlineQueryService inlineQueryService, ILogger<UpdateController> logger)
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
}
