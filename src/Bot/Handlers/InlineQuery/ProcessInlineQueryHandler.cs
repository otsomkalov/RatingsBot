using Bot.Requests.InlineQuery;
using Bot.Requests.Item;
using Bot.Requests.Rating;
using Core.Requests.User;
using Data;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types.InlineQueryResults;

namespace Bot.Handlers.InlineQuery;

public class ProcessInlineQueryHandler : IRequestHandler<ProcessInlineQuery, Unit>
{
    private readonly ITelegramBotClient _bot;
    private readonly AppDbContext _context;
    private readonly IMediator _mediator;

    public ProcessInlineQueryHandler(ITelegramBotClient bot, AppDbContext context, IMediator mediator)
    {
        _bot = bot;
        _context = context;
        _mediator = mediator;
    }

    public async Task<Unit> Handle(ProcessInlineQuery request, CancellationToken cancellationToken)
    {
        var inlineQuery = request.InlineQuery;

        await _mediator.Send(new CreateUserIfNotExists(inlineQuery.From.Id, inlineQuery.From.FirstName), cancellationToken);

        var items = await _context.Items
            .Include(i => i.Category)
            .Include(i => i.Place)
            .Include(i => i.Manufacturer)
            .Include(i => i.Ratings)
            .ThenInclude(r => r.User)
            .Where(i =>
                EF.Functions.ILike(i.Name, $"%{inlineQuery.Query}%") ||
                EF.Functions.ILike(i.Category.Name, $"%{inlineQuery.Query}%") ||
                EF.Functions.ILike(i.Place.Name, $"%{inlineQuery.Query}%") ||
                EF.Functions.ILike(i.Manufacturer.Name, $"%{inlineQuery.Query}%"))
            .Take(Constants.Telegram.MaximumInlineResults)
            .ToListAsync(cancellationToken);

        var articles = await Task.WhenAll(items.Select(async item => await ItemToArticleAsync(item, inlineQuery, cancellationToken)));

        await _bot.AnswerInlineQueryAsync(inlineQuery.Id, articles, cancellationToken: cancellationToken);

        return Unit.Value;
    }

    private async Task<InlineQueryResultArticle> ItemToArticleAsync(Core.Models.Item item, Telegram.Bot.Types.InlineQuery inlineQuery,
        CancellationToken cancellationToken)
    {
        var currentUserRating = item.Ratings.FirstOrDefault(r => r.UserId == inlineQuery.From.Id);

        var avgRating = item.Ratings.Any()
            ? CalculateAverageRating(item.Ratings)
            : 0;

        var currentRatingString = currentUserRating != null
            ? StringHelpers.CreateStarsString(currentUserRating.Value)
            : "-";

        var avgRatingString = avgRating != 0
            ? StringHelpers.CreateStarsString(avgRating)
            : "-";

        var titleParts = new[]
        {
            item.Category?.Name, item.Manufacturer?.Name, item.Name, item.Place?.Name
        };

        var title = string.Join(" ", titleParts.Where(part => !string.IsNullOrEmpty(part)));
        var messageText = await _mediator.Send(new GetItemMessageText(item), cancellationToken);
        var messageContent = new InputTextMessageContent(messageText);
        var replyMarkup = await _mediator.Send(new GetRatingsMarkup(item.Id), cancellationToken);

        var article = new InlineQueryResultArticle(item.Id.ToString(), title, messageContent)
        {
            Description = $"{currentRatingString}/{avgRatingString}",
            ReplyMarkup = replyMarkup
        };

        return article;
    }

    private static int CalculateAverageRating(IReadOnlyCollection<Core.Models.Rating> ratings)
    {
        return (int) Math.Round(ratings.Sum(r => r.Value) / (double) ratings.Count, MidpointRounding.ToPositiveInfinity);
    }
}