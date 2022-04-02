using System.Threading;
using System.Threading.Tasks;
using Bot.Handlers.CallbackQuery;
using Bot.Requests.Manufacturer;
using Bot.Requests.Message;
using Bot.Requests.Place;
using Bot.Resources;
using Core.Requests.Item;
using MediatR;
using Microsoft.Extensions.Localization;
using NSubstitute;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Xunit;

namespace Bot.Tests.Handlers.CallbackQuery;

public class ProcessManufacturerCommandTests
{
    private readonly ITelegramBotClient _bot;
    private readonly IMediator _mediator;

    private readonly ProcessManufacturerCommandHandler _sut;

    public ProcessManufacturerCommandTests()
    {
        _mediator = Substitute.For<IMediator>();
        _bot = Substitute.For<ITelegramBotClient>();

        var localizer = Substitute.For<IStringLocalizer<Messages>>();

        _sut = new(_mediator, _bot, localizer);
    }

    [Theory]
    [InlineData("1|m|1")]
    [InlineData("1|m|null")]
    public async Task RegularMessageCallbackQuery_SetItemManufacturer_Works(string data)
    {
        // Arrange

        var callbackQuery = new Telegram.Bot.Types.CallbackQuery
        {
            Data = data,
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

        await _mediator.Received().Send(Arg.Any<SetItemManufacturer>(), Arg.Any<CancellationToken>());
        await _mediator.Received().Send(Arg.Any<GetPlacesMarkup>(), Arg.Any<CancellationToken>());
        await _bot.Received().MakeRequestAsync(Arg.Any<EditMessageTextRequest>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("1|m|0")]
    [InlineData("1|m|-1")]
    public async Task RegularMessageCallbackQuery_RefreshManufacturers_Works(string data)
    {
        // Arrange

        var callbackQuery = new Telegram.Bot.Types.CallbackQuery
        {
            Data = data,
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

        await _mediator.Received().Send(Arg.Any<GetManufacturersMarkup>(), Arg.Any<CancellationToken>());
        await _mediator.Received().Send(Arg.Any<EditMessageReplyMarkup>(), Arg.Any<CancellationToken>());
    }
}