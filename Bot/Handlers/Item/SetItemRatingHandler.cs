using Bot.Commands.Item;
using Bot.Commands.Rating;
using Bot.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Telegram.Bot.Exceptions;

namespace Bot.Handlers.Item;

public class SetItemRatingHandler : AsyncRequestHandler<SetItemRating>
{
    private readonly ITelegramBotClient _bot;
    private readonly IStringLocalizer<Messages> _localizer;
    private readonly AppDbContext _context;
    private readonly IMediator _mediator;

    public SetItemRatingHandler(ITelegramBotClient bot, IStringLocalizer<Messages> localizer, AppDbContext context, IMediator mediator)
    {
        _bot = bot;
        _localizer = localizer;
        _context = context;
        _mediator = mediator;
    }

    protected override async Task Handle(SetItemRating request, CancellationToken cancellationToken)
    {
        var (callbackQuery, entityId, itemId) = request;

        if (entityId.HasValue)
        {
            var rating = await _context.Ratings
                .FirstOrDefaultAsync(r => r.UserId == callbackQuery.From.Id && r.ItemId == itemId,
                cancellationToken);

            if (rating != null)
            {
                rating.Value = entityId.Value;

                _context.Update(rating);
            }
            else
            {
                rating = new()
                {
                    ItemId = itemId,
                    UserId = callbackQuery.From.Id,
                    Value = entityId.Value
                };

                await _context.AddAsync(rating, cancellationToken);
            }

            await _context.SaveChangesAsync(cancellationToken);

            await _bot.AnswerCallbackQueryAsync(callbackQuery.Id, _localizer[nameof(Messages.Recorded)],
                cancellationToken: cancellationToken);
        }

        var item = await _context.Items
            .Include(i => i.Ratings)
            .ThenInclude(r => r.User)
            .Include(i => i.Place)
            .Include(i => i.Category)
            .Include(i => i.Manufacturer)
            .FirstOrDefaultAsync(i => i.Id == itemId, cancellationToken);

        var messageText = await _mediator.Send(new GetItemMessageText(item), cancellationToken);

        try
        {
            var ratingsMarkup = await _mediator.Send(new GetRatingsMarkup(item.Id), cancellationToken);

            if (callbackQuery.InlineMessageId != null)
            {
                await _bot.EditMessageTextAsync(callbackQuery.InlineMessageId,
                    messageText,
                    replyMarkup: ratingsMarkup,
                    cancellationToken: cancellationToken);
            }
            else
            {
                await _bot.EditMessageTextAsync(new(callbackQuery.From.Id),
                    callbackQuery.Message.MessageId,
                    messageText,
                    replyMarkup: ratingsMarkup,
                    cancellationToken: cancellationToken);
            }
        }
        catch (MessageIsNotModifiedException)
        {
            await _bot.AnswerCallbackQueryAsync(callbackQuery.Id,
                _localizer[nameof(Messages.Refreshed)], cancellationToken: cancellationToken);
        }
    }
}