using Bot.Constants;
using Bot.Requests.CallbackQuery;
using Bot.Requests.InlineKeyboardMarkup;
using Bot.Requests.Message;
using Bot.Resources;
using Core.Requests.Item;
using Microsoft.Extensions.Localization;

namespace Bot.Handlers.CallbackQuery;

public class ProcessCategoryCommandHandler : IRequestHandler<ProcessCategoryCommand, Unit>
{
    private readonly ITelegramBotClient _bot;
    private readonly IStringLocalizer<Messages> _localizer;
    private readonly IMediator _mediator;

    public ProcessCategoryCommandHandler(IMediator mediator, ITelegramBotClient bot, IStringLocalizer<Messages> localizer)
    {
        _mediator = mediator;
        _bot = bot;
        _localizer = localizer;
    }

    public async Task<Unit> Handle(ProcessCategoryCommand request, CancellationToken cancellationToken)
    {
        var callbackQueryData = request.CallbackQueryQueryData;

        if (callbackQueryData.CategoryId == 0)
        {
            var categoriesMarkup = await _mediator.Send(
                new GetInlineKeyboardMarkup(callbackQueryData.ItemId, ReplyMarkup.Category, callbackQueryData.Page),
                cancellationToken);

            await _mediator.Send(new EditMessageReplyMarkup(callbackQueryData, categoriesMarkup), cancellationToken);

            return Unit.Value;
        }

        await _mediator.Send(new SetItemCategory(callbackQueryData.ItemId, callbackQueryData.CategoryId), cancellationToken);

        var manufacturersMarkup = await _mediator.Send(new GetManufacturersMarkup(callbackQueryData.ItemId), cancellationToken);

        await _bot.EditMessageTextAsync(new(callbackQueryData.UserId),
            callbackQueryData.MessageId.Value,
            _localizer[nameof(Messages.SelectManufacturer)],
            replyMarkup: manufacturersMarkup,
            cancellationToken: cancellationToken);

        return Unit.Value;
    }
}