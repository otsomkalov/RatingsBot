using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RatingsBot.Data;
using RatingsBot.Models;
using RatingsBot.Options;
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

        public MessageService(ITelegramBotClient bot, IOptions<TelegramOptions> telegramOptions, AppDbContext context)
        {
            _bot = bot;
            _context = context;
            _telegramOptions = telegramOptions.Value;
        }

        public async Task HandleAsync(Message message)
        {
            if (message.Text.StartsWith("/start", StringComparison.InvariantCultureIgnoreCase))
            {
                await _context.AddAsync(new User
                {
                    TelegramId = message.From.Id
                });

                await _context.SaveChangesAsync();

                await _bot.SendTextMessageAsync(
                    new ChatId(message.From.Id),
                    )

                return;
            }

            if (message.Text.StartsWith(NewItemCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                var name = message.Text[NewItemCommand.Length..];

                await _context.AddAsync(new Item
                {
                    Name = name
                });

                var users = await _context.Users
                    .AsNoTracking()
                    .ToListAsync();

                foreach (var user in users)
                {
                    await _bot.SendTextMessageAsync(
                        new(user.Id),
                        "",
                        replyMarkup: )
                }
            }
        }
    }
}
