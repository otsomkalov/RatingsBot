using Microsoft.Extensions.Localization;
using RatingsBot.Commands.Message;
using RatingsBot.Resources;

namespace RatingsBot.Handlers.Message;

public class CreateCategoryCommandHandler : AsyncRequestHandler<CreateCategoryCommand>
{
    private readonly ITelegramBotClient _bot;
    private readonly CategoryService _categoryService;
    private readonly IStringLocalizer<Messages> _localizer;

    public CreateCategoryCommandHandler(CategoryService categoryService, ITelegramBotClient bot, IStringLocalizer<Messages> localizer)
    {
        _categoryService = categoryService;
        _bot = bot;
        _localizer = localizer;
    }

    protected override async Task Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var message = request.Message;

        await _categoryService.AddAsync(message.Text.Trim());

        await _bot.SendTextMessageAsync(new(message.From.Id),
            _localizer[Messages.Created],
            replyToMessageId: message.MessageId,
            replyMarkup: ReplyKeyboardMarkupHelpers.GetStartReplyKeyboardMarkup(),
            cancellationToken: cancellationToken);
    }
}