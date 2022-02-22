using Bot.Commands.Manufacturer;
using Bot.Resources;
using Microsoft.Extensions.Localization;

namespace Bot.Handlers.Manufacturer;

public class CreateManufacturerHandler : AsyncRequestHandler<CreateManufacturer>
{
    private readonly ITelegramBotClient _bot;
    private readonly AppDbContext _context;
    private readonly IStringLocalizer<Messages> _localizer;

    public CreateManufacturerHandler(AppDbContext context, ITelegramBotClient bot, IStringLocalizer<Messages> localizer)
    {
        _context = context;
        _bot = bot;
        _localizer = localizer;
    }

    protected override async Task Handle(CreateManufacturer request, CancellationToken cancellationToken)
    {
        var message = request.Message;

        await _context.Manufacturers.AddAsync(new()
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