using Bot.Commands.Category;
using Bot.Resources;
using Microsoft.Extensions.Localization;

namespace Bot.Handlers.Category;

public class CreateCategoryHandler : AsyncRequestHandler<CreateCategory>
{
    private readonly ITelegramBotClient _bot;
    private readonly IStringLocalizer<Messages> _localizer;
    private readonly AppDbContext _context;

    public CreateCategoryHandler(ITelegramBotClient bot, IStringLocalizer<Messages> localizer, AppDbContext context)
    {
        _bot = bot;
        _localizer = localizer;
        _context = context;
    }

    protected override async Task Handle(CreateCategory request, CancellationToken cancellationToken)
    {
        var message = request.Message;

        await _context.Categories.AddAsync(new()
        {
            Name = message.Text.Trim()
        }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        await _bot.SendTextMessageAsync(new(message.From.Id),
            _localizer[nameof(Messages.Created)],
            replyToMessageId: message.MessageId,
            replyMarkup: ReplyKeyboardMarkupHelpers.GetStartReplyKeyboardMarkup(),
            cancellationToken: cancellationToken);
    }
}