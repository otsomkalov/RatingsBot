using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using RatingsBot.Constants;
using RatingsBot.Helpers;
using RatingsBot.Resources;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RatingsBot.Services
{
    public class MessageService
    {
        private const string StartCommand = "/start";
        private const string NewItemCommand = "/newitem";
        private const string NewPlaceCommand = "/newplace";
        private const string NewCategoryCommand = "/newcategory";

        private readonly ITelegramBotClient _bot;
        private readonly IStringLocalizer<Messages> _localizer;
        private readonly CategoryService _categoryService;
        private readonly PlaceService _placeService;
        private readonly UserService _userService;
        private readonly ItemService _itemService;

        public MessageService(ITelegramBotClient bot, IStringLocalizer<Messages> localizer,
            CategoryService categoryService, PlaceService placeService, UserService userService, ItemService itemService)
        {
            _bot = bot;
            _localizer = localizer;
            _categoryService = categoryService;
            _placeService = placeService;
            _userService = userService;
            _itemService = itemService;
        }

        public Task HandleAsync(Message message)
        {
            if (message.Text.StartsWithCI(StartCommand))
            {
                return HandleStartCommandAsync(message);
            }

            if (message.Text.StartsWithCI(NewCategoryCommand))
            {
                return HandleNewCategoryCommandAsync(message);
            }

            if (message.Text.StartsWithCI(NewPlaceCommand))
            {
                return HandleNewPlaceCommandAsync(message);
            }

            if (message.Text.StartsWithCI(NewItemCommand))
            {
                return HandleNewItemCommandAsync(message);
            }

            return Task.CompletedTask;
        }

        private async Task HandleNewItemCommandAsync(Message message)
        {
            if (message.Text.Length == NewItemCommand.Length)
            {
                await _bot.SendTextMessageAsync(new(message.From.Id),
                    _localizer[ResourcesNames.NewItemCommand]);

                return;
            }

            var categories = await _categoryService.ListAsync();

            var newItemId = await _itemService.AddAsync(message.Text[NewItemCommand.Length..].Trim());

            await _bot.SendTextMessageAsync(new(message.From.Id),
                _localizer[ResourcesNames.Category],
                replyMarkup: ReplyMarkupHelpers.GetCategoriesMarkup(newItemId, categories));
        }

        private async Task HandleNewPlaceCommandAsync(Message message)
        {
            if (message.Text.Length == NewPlaceCommand.Length)
            {
                await _bot.SendTextMessageAsync(new(message.From.Id),
                    _localizer[ResourcesNames.NewPlaceCommand]);

                return;
            }

            await _placeService.AddAsync(message.Text[NewPlaceCommand.Length..].Trim());

            await _bot.SendTextMessageAsync(new(message.From.Id),
                _localizer[ResourcesNames.Created]);
        }

        private async Task HandleNewCategoryCommandAsync(Message message)
        {
            if (message.Text.Length == NewCategoryCommand.Length)
            {
                await _bot.SendTextMessageAsync(new(message.From.Id),
                    _localizer[ResourcesNames.NewCategoryCommand]);

                return;
            }

            await _categoryService.AddAsync(message.Text[NewCategoryCommand.Length..].Trim());

            await _bot.SendTextMessageAsync(new(message.From.Id),
                _localizer[ResourcesNames.Created]);
        }

        private async Task HandleStartCommandAsync(Message message)
        {
            await _userService.CreateIfNotExistsAsync(message.From.Id);

            await _bot.SendTextMessageAsync(new(message.From.Id),
                _localizer[ResourcesNames.Welcome]);
        }
    }
}
