using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Localization;
using RatingsBot.Commands.Message;
using RatingsBot.Constants;
using RatingsBot.Resources;
using RatingsBot.Services;
using Telegram.Bot;

namespace RatingsBot.Handlers.Message;

public class NewCategoryCommandHandler : AsyncRequestHandler<NewCategoryCommand>
{
    private readonly ITelegramBotClient _bot;
    private readonly IStringLocalizer<Messages> _localizer;
    private readonly CategoryService _categoryService;

    public NewCategoryCommandHandler(ITelegramBotClient bot, CategoryService categoryService, IStringLocalizer<Messages> localizer)
    {
        _bot = bot;
        _categoryService = categoryService;
        _localizer = localizer;
    }

    protected override async Task Handle(NewCategoryCommand request, CancellationToken cancellationToken)
    {
        var message = request.Message;

        if (message.Text.Length == Constants.Commands.NewCategory.Length)
        {
            await _bot.SendTextMessageAsync(new(message.From.Id),
                _localizer[ResourcesNames.NewCategoryCommand], cancellationToken: cancellationToken);

            return;
        }

        await _categoryService.AddAsync(message.Text[Constants.Commands.NewCategory.Length..].Trim());

        await _bot.SendTextMessageAsync(new(message.From.Id),
            _localizer[ResourcesNames.Created], cancellationToken: cancellationToken);
    }
}