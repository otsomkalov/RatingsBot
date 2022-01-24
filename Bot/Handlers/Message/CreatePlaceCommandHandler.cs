using Bot.Commands.Message;
using Bot.Models;
using Bot.Resources;
using Microsoft.Extensions.Localization;

namespace Bot.Handlers.Message;

public class CreatePlaceCommandHandler : AsyncRequestHandler<CreatePlaceCommand>
{
    private readonly ITelegramBotClient _bot;
    private readonly IStringLocalizer<Messages> _localizer;
    private readonly AppDbContext _context;

    public CreatePlaceCommandHandler(ITelegramBotClient bot, IStringLocalizer<Messages> localizer, AppDbContext context)
    {
        _bot = bot;
        _localizer = localizer;
        _context = context;
    }

    protected override async Task Handle(CreatePlaceCommand request, CancellationToken cancellationToken)
    {
        var message = request.Message;

        await _context.AddAsync(new Place
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