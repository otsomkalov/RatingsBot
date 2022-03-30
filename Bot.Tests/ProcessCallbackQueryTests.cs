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
using Xunit;

namespace Bot.Tests;

public class ProcessCallbackQueryTests
{
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

        var processCallbackQueryHandler = GetHandler();

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

        await processCallbackQueryHandler.Handle(callbackQuery, CancellationToken.None);

        // Assert
        // Doesn't throw an exception
    }

    private static ProcessCallbackQueryHandler GetHandler()
    {
        var mediatorMock = new Mock<IMediator>();

        mediatorMock.Setup(m => m.Send(It.IsAny<GetItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Item
            {
                Id = 1
            });

        var telegramClientMock = new Mock<ITelegramBotClient>();

        var localizerMock = new Mock<IStringLocalizer<Messages>>();

        var processCallbackQueryHandler =
            new ProcessCallbackQueryHandler(mediatorMock.Object, telegramClientMock.Object, localizerMock.Object);

        return processCallbackQueryHandler;
    }

    [Theory]
    [InlineData("1|r|5")]
    [InlineData("1|r|null")]
    public async Task InlineMessageCallbackQuery_Works(string data)
    {
        // Arrange

        var processCallbackQueryHandler = GetHandler();

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

        await processCallbackQueryHandler.Handle(callbackQuery, CancellationToken.None);

        // Assert
        // Doesn't throw an exception
    }
}