using System.Threading;
using System.Threading.Tasks;
using Bot.Handlers.CallbackQuery;
using Bot.Requests.CallbackQuery;
using MediatR;
using NSubstitute;
using Xunit;

namespace Bot.Tests.Handlers.CallbackQuery;

public class ProcessCallbackQueryTests
{
    private readonly IMediator _mediator;
    private readonly ProcessCallbackQueryHandler _sut;

    public ProcessCallbackQueryTests()
    {
        _mediator = Substitute.For<IMediator>();

        _sut = new(_mediator);
    }

    [Theory]
    [InlineData("1|c|1")]
    [InlineData("1|c|0")]
    [InlineData("1|c|null")]
    [InlineData("1|c|-1")]
    public async Task RegularMessageCallbackQuery_ProcessCategoryCommand_IsCalled(string data)
    {
        // Arrange

        var callbackQuery = new Telegram.Bot.Types.CallbackQuery
        {
            Data = data,
            From = new()
        };

        // Act

        await _sut.Handle(new(callbackQuery), CancellationToken.None);

        // Assert

        await _mediator.Received().Send(Arg.Any<ProcessCategoryCommand>());
    }

    [Theory]
    [InlineData("1|m|1")]
    [InlineData("1|m|0")]
    [InlineData("1|m|null")]
    [InlineData("1|m|-1")]
    public async Task RegularMessageCallbackQuery_ProcessManufacturerCommand_IsCalled(string data)
    {
        // Arrange

        var callbackQuery = new Telegram.Bot.Types.CallbackQuery
        {
            Data = data,
            From = new()
        };

        // Act

        await _sut.Handle(new(callbackQuery), CancellationToken.None);

        // Assert

        await _mediator.Received().Send(Arg.Any<ProcessManufacturerCommand>());
    }

    [Theory]
    [InlineData("1|p|1")]
    [InlineData("1|p|0")]
    [InlineData("1|p|null")]
    [InlineData("1|p|-1")]
    public async Task RegularMessageCallbackQuery_ProcessPlaceCommand_IsCalled(string data)
    {
        // Arrange

        var callbackQuery = new Telegram.Bot.Types.CallbackQuery
        {
            Data = data,
            From = new()
        };

        // Act

        await _sut.Handle(new(callbackQuery), CancellationToken.None);

        // Assert

        await _mediator.Received().Send(Arg.Any<ProcessPlaceCommand>());
    }

    [Theory]
    [InlineData("1|r|1")]
    [InlineData("1|r|0")]
    [InlineData("1|r|null")]
    public async Task RegularMessageCallbackQuery_ProcessRatingCommand_IsCalled(string data)
    {
        // Arrange

        var callbackQuery = new Telegram.Bot.Types.CallbackQuery
        {
            Data = data,
            From = new()
        };

        // Act

        await _sut.Handle(new(callbackQuery), CancellationToken.None);

        // Assert

        await _mediator.Received().Send(Arg.Any<ProcessRatingCommand>());
    }
}