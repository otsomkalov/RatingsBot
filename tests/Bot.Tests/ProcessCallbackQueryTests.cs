using System.Threading;
using System.Threading.Tasks;
using Bot.Commands.CallbackQuery;
using Bot.Handlers.CallbackQuery;
using Bot.Resources;
using Core.Commands.Item;
using Core.Models;
using MediatR;
using Microsoft.Extensions.Localization;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Requests;
using Xunit;

namespace Bot.Tests;

public class ProcessCallbackQueryTests
{
    private readonly ProcessCallbackQueryHandler _sut;
    private readonly Mock<ITelegramBotClient> _telegramClient;

    public ProcessCallbackQueryTests()
    {
        var mediator = new Mock<IMediator>();
        _telegramClient = new();
        var localizer = new Mock<IStringLocalizer<Messages>>();

        mediator.Setup(m => m.Send(It.IsAny<GetItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Item
            {
                Id = 1
            });

        _sut = new(mediator.Object, _telegramClient.Object, localizer.Object);
    }

    [Theory]
    [InlineData("1|c|1")]
    [InlineData("1|c|null")]
    [InlineData("1|p|1")]
    [InlineData("1|p|null")]
    [InlineData("1|p|-1")]
    [InlineData("1|m|1")]
    [InlineData("1|m|null")]
    [InlineData("1|m|-1")]
    [InlineData("1|r|5")]
    [InlineData("1|r|null")]
    public async Task RegularMessageCallbackQuery_Works(string data)
    {
        // Arrange

        var callbackQuery = new ProcessCallbackQuery(new()
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
        });

        // Act

        await _sut.Handle(callbackQuery, CancellationToken.None);

        // Assert
        // Doesn't throw an exception
    }

    [Theory]
    [InlineData("1|r|5")]
    [InlineData("1|r|null")]
    public async Task InlineMessageCallbackQuery_Works(string data)
    {
        // Arrange

        var callbackQuery = new ProcessCallbackQuery(new()
        {
            Data = data,
            From = new()
            {
                Id = 1,
                FirstName = "test"
            },
            InlineMessageId = "test"
        });

        // Act

        await _sut.Handle(callbackQuery, CancellationToken.None);

        // Assert
        // Doesn't throw an exception
    }

    [Theory]
    [InlineData("1|r|-1")]
    [InlineData("1|p|-1")]
    [InlineData("1|c|null")]
    [InlineData("1|m|-1")]
    public async Task InlineMessageCallbackQueryRefreshCommand_Works(string data)
    {
        // Arrange

        _telegramClient.Setup(m =>
                m.MakeRequestAsync(It.IsAny<EditMessageReplyMarkupRequest>(), It.IsAny<CancellationToken>()))
            .Throws(new ApiRequestException(string.Empty));

        _telegramClient.Setup(m =>
                m.MakeRequestAsync(It.IsAny<EditMessageTextRequest>(), It.IsAny<CancellationToken>()))
            .Throws(new ApiRequestException(string.Empty));

        var callbackQuery = new ProcessCallbackQuery(new()
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
        });

        // Act

        await _sut.Handle(callbackQuery, CancellationToken.None);

        // Assert

        _telegramClient.Verify(m => m.MakeRequestAsync(It.IsAny<AnswerCallbackQueryRequest>(), It.IsAny<CancellationToken>()));
    }
}