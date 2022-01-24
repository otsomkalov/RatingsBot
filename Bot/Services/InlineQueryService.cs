using Bot.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Telegram.Bot.Types;

namespace Bot.Services;

public class InlineQueryService
{
    private readonly ITelegramBotClient _bot;
    private readonly IStringLocalizer<Messages> _localizer;
    private readonly UserService _userService;
    private readonly AppDbContext _context;

    public InlineQueryService(ITelegramBotClient bot, IStringLocalizer<Messages> localizer, UserService userService, AppDbContext context)
    {
        _bot = bot;
        _localizer = localizer;
        _userService = userService;
        _context = context;
    }

    public async Task HandleAsync(InlineQuery inlineQuery)
    {
        await _userService.CreateIfNotExistsAsync(inlineQuery.From.Id);

        var items = await _context.Items
            .Include(i => i.Category)
            .Include(i => i.Place)
            .Include(i => i.Ratings)
            .ThenInclude(r => r.User)
            .Where(i =>
                EF.Functions.ILike(i.Name, $"%{inlineQuery.Query}%") ||
                EF.Functions.ILike(i.Category.Name, $"%{inlineQuery.Query}%") ||
                EF.Functions.ILike(i.Place.Name, $"%{inlineQuery.Query}%"))
            .Take(Constants.Telegram.MaximumInlineResults)
            .ToListAsync();

        var itemsArticles = items
            .Select(item => InlineQueryResultHelpers.GetItemQueryResult(item, inlineQuery,
                _localizer[nameof(Messages.ItemMessageTemplate)], _localizer[nameof(Messages.RatingLineTemplate)]));

        await _bot.AnswerInlineQueryAsync(inlineQuery.Id, itemsArticles);
    }
}