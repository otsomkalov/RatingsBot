using System;
using System.Text.RegularExpressions;
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
        private const string NewItemCommand = "/newitem ";

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
            if (message.Text.StartsWith("/start", StringComparison.InvariantCultureIgnoreCase))
            {
                await _context.AddAsync(new User
                {
                    Id = message.From.Id
                });

                await _context.SaveChangesAsync();

                await _bot.SendTextMessageAsync(
                    new(message.From.Id),
                    _localizer[ResourcesNames.Welcome]);

                return;
            }

            if (message.Text.StartsWith(NewItemCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                var name = message.Text[NewItemCommand.Length..];

                var createdItem = await _context.AddAsync(new Item
                {
                    Name = name
                });

                await _context.SaveChangesAsync();

                var users = await _context.Users
                    .AsNoTracking()
                    .ToListAsync();

                foreach (var user in users)
                {
                    await _bot.SendTextMessageAsync(new(user.Id),
                        name,
                        replyMarkup: ReplyMarkupHelpers.GetRatingsMarkup(createdItem.Entity.Id));
                }
            }
        }
    }
}
