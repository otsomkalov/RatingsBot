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
        private readonly ILogger<UpdateController> _logger;

        public UpdateController(MessageService messageService, CallbackQueryService callbackQueryService, InlineQueryService inlineQueryService, ILogger<UpdateController> logger)
        {
            _messageService = messageService;
            _callbackQueryService = callbackQueryService;
            _inlineQueryService = inlineQueryService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> HandleUpdateAsync(Update update)
        {
            try
            {
                if (update.Type == UpdateType.Message)
                {
                    await _messageService.HandleAsync(update.Message);
                }

                if (update.Type == UpdateType.CallbackQuery)
                {
                    await _callbackQueryService.HandleAsync(update.CallbackQuery);
                }

                if (update.Type == UpdateType.InlineQuery)
                {
                    await _inlineQueryService.HandleAsync(update.InlineQuery);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, string.Empty);
            }

            return Ok();
        }
    }
}
