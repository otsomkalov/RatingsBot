using System.Threading;
using System.Threading.Tasks;
using Bot.Handlers.CallbackQuery;
using Bot.Requests.Item;
using Bot.Requests.Message;
using Bot.Requests.Place;
using Bot.Requests.Rating;
using Bot.Resources;
using Core.Requests.Item;
using MediatR;
using Microsoft.Extensions.Localization;
using NSubstitute;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Xunit;

namespace Bot.Tests.Handlers.CallbackQuery;

public class ProcessPlaceCommandTests
{
    private readonly ITelegramBotClient _bot;
    private readonly IMediator _mediator;

    private readonly ProcessPlaceCommandHandler _sut;

    public ProcessPlaceCommandTests()
    {
        _mediator = Substitute.For<IMediator>();
        _bot = Substitute.For<ITelegramBotClient>();

        var localizer = Substitute.For<IStringLocalizer<Messages>>();

        _sut = new(_mediator, _bot);
    }

    [Theory]
    [InlineData("1|p|1")]
    [InlineData("1|p|null")]
    public async Task RegularMessageCallbackQuery_SetItemPlace_Works(string data)
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

        await _mediator.Received().Send(Arg.Any<SetItemPlace>(), Arg.Any<CancellationToken>());
        await _mediator.Received().Send(Arg.Any<GetItem>(), Arg.Any<CancellationToken>());
        await _mediator.Received().Send(Arg.Any<GetRatingsMarkup>(), Arg.Any<CancellationToken>());
        await _mediator.Received().Send(Arg.Any<GetItemMessageText>(), Arg.Any<CancellationToken>());
        await _bot.Received().MakeRequestAsync(Arg.Any<EditMessageTextRequest>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("1|p|0")]
    [InlineData("1|p|-1")]
    public async Task RegularMessageCallbackQuery_RefreshPlaces_Works(string data)
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

        await _mediator.Received().Send(Arg.Any<GetPlacesMarkup>(), Arg.Any<CancellationToken>());
        await _mediator.Received().Send(Arg.Any<EditMessageReplyMarkup>(), Arg.Any<CancellationToken>());
    }
}