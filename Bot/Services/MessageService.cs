using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using RatingsBot.Constants;
using RatingsBot.Data;
using RatingsBot.Helpers;
using RatingsBot.Models;
using RatingsBot.Options;
using RatingsBot.Resources;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = RatingsBot.Models.User;

namespace RatingsBot.Services
{
    public class MessageService
    {
        private const string StartCommand = "/start";
        private const string NewItemCommand = "/newitem";
        private const string NewPlaceCommand = "/newplace";
        private const string NewCategoryCommand = "/newcategory";

        private readonly ITelegramBotClient _bot;
        private readonly TelegramOptions _telegramOptions;
        private readonly AppDbContext _context;
        private readonly IStringLocalizer<Messages> _localizer;

        public MessageService(ITelegramBotClient bot, IOptions<TelegramOptions> telegramOptions, AppDbContext context, IStringLocalizer<Messages> localizer)
        {
            _bot = bot;
            _context = context;
            _localizer = localizer;
            _telegramOptions = telegramOptions.Value;
        }

        public async Task HandleAsync(Message message)
        {
            if (message.Text.StartsWithCI(StartCommand))
            {
                if (!await _context.Users.AnyAsync(u => u.Id == message.From.Id))
                {
                    await _context.AddAsync(new User
                    {
                        Id = message.From.Id,
                        FirstName = message.From.FirstName
                    });

                    await _context.SaveChangesAsync();
                }

                await _bot.SendTextMessageAsync(
                    new(message.From.Id),
                    _localizer[ResourcesNames.Welcome]);

                return;
            }

            if (message.Text.StartsWithCI(NewCategoryCommand))
            {
                if (message.Text.Length == NewCategoryCommand.Length)
                {
                    await _bot.SendTextMessageAsync(new(message.From.Id),
                        _localizer[ResourcesNames.NewCategoryCommand]);

                    return;
                }

                var categoryName = message.Text[NewCategoryCommand.Length..].Trim();

                await _context.AddAsync(new Category
                {
                    Name = categoryName
                });

                await _context.SaveChangesAsync();

                await _bot.SendTextMessageAsync(new(message.From.Id),
                    _localizer[ResourcesNames.Created]);
            }

            if (message.Text.StartsWithCI(NewPlaceCommand))
            {
                if (message.Text.Length == NewPlaceCommand.Length)
                {
                    await _bot.SendTextMessageAsync(new(message.From.Id),
                        _localizer[ResourcesNames.NewPlaceCommand]);

                    return;
                }

                var placeName = message.Text[NewPlaceCommand.Length..].Trim();

                await _context.AddAsync(new Place
                {
                    Name = placeName
                });

                await _context.SaveChangesAsync();

                await _bot.SendTextMessageAsync(new(message.From.Id),
                    _localizer[ResourcesNames.Created]);
            }

            if (message.Text.StartsWithCI(NewItemCommand))
            {
                if (message.Text.Length == NewItemCommand.Length)
                {
                    await _bot.SendTextMessageAsync(new(message.From.Id),
                        _localizer[ResourcesNames.NewItemCommand]);

                    return;
                }

                var categories = await _context.Categories.AsNoTracking().ToListAsync();

                var name = message.Text[NewItemCommand.Length..].Trim();

                var newItem = new Item
                {
                    Name = name
                };
                await _context.AddAsync(newItem);
                await _context.SaveChangesAsync();

                await _bot.SendTextMessageAsync(
                    new(message.From.Id),
                    _localizer[ResourcesNames.Category],
                    replyMarkup: ReplyMarkupHelpers.GetCategoriesMarkup(newItem.Id, categories));

            }
        }
    }
}
