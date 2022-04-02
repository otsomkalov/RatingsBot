using System.Threading;
using System.Threading.Tasks;
using Bot.Handlers.CallbackQuery;
using Bot.Requests.Category;
using Bot.Requests.Manufacturer;
using Bot.Requests.Message;
using Bot.Resources;
using Core.Requests.Item;
using MediatR;
using Microsoft.Extensions.Localization;
using NSubstitute;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Xunit;

namespace Bot.Tests.Handlers.CallbackQuery;

public class ProcessCategoryCommandTests
{
    private readonly ProcessCategoryCommandHandler _sut;
    private readonly IMediator _mediator;
    private readonly ITelegramBotClient _bot;

    public ProcessCategoryCommandTests()
    {
        _mediator = Substitute.For<IMediator>();
        _bot = Substitute.For<ITelegramBotClient>();

        var lozalizer = Substitute.For<IStringLocalizer<Messages>>();

        _sut = new(_mediator, _bot, lozalizer);
    }

    [Fact]
    public async Task RegularMessageCallbackQuery_SetCategory_Works()
    {
        // Arrange

        var callbackQuery = new Telegram.Bot.Types.CallbackQuery
        {
            Data = "1|c|1",
            From = new()
            {
                Id = 1,
                FirstName = "test"
            },
            Message = new()
            {
                MessageId = 1
            }
        };

        // Act

        await _sut.Handle(new(new(callbackQuery)), CancellationToken.None);

        // Assert

        await _mediator.Received().Send(Arg.Any<SetItemCategory>(), Arg.Any<CancellationToken>());
        await _mediator.Received().Send(Arg.Any<GetManufacturersMarkup>(), Arg.Any<CancellationToken>());
        await _bot.Received().MakeRequestAsync(Arg.Any<EditMessageTextRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RegularMessageCallbackQuery_RefreshCategories_Works()
    {
        // Arrange

        var callbackQuery = new Telegram.Bot.Types.CallbackQuery
        {
            Data = "1|c|null",
            From = new()
            {
                Id = 1,
                FirstName = "test"
            },
            Message = new()
            {
                MessageId = 1
            }
        };

        // Act

        await _sut.Handle(new(new(callbackQuery)), CancellationToken.None);

        // Assert

        await _mediator.Received().Send(Arg.Any<GetCategoriesMarkup>(), Arg.Any<CancellationToken>());
        await _mediator.Received().Send(Arg.Any<EditMessageReplyMarkup>(), Arg.Any<CancellationToken>());
    }
}