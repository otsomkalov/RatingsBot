using System.Collections.Generic;
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
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
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
    public async Task ProcessCallbackQuery_Works(string data)
    {
        // Arrange

        var mediatorMock = new Mock<IMediator>();

        mediatorMock.Setup(m => m.Send(It.IsAny<GetItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Item
            {
                Id = 1
            });

        var telegramClientMock = new Mock<ITelegramBotClient>();

        telegramClientMock.Setup(m => m.SendTextMessageAsync(It.IsAny<ChatId>(), It.IsAny<string>(), It.IsAny<ParseMode>(),
                It.IsAny<IEnumerable<MessageEntity>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<bool>(),
                It.IsAny<IReplyMarkup>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Message());

        telegramClientMock.Setup(m => m.EditMessageTextAsync(It.IsAny<ChatId>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<ParseMode>(),
                It.IsAny<IEnumerable<MessageEntity>>(), It.IsAny<bool>(), It.IsAny<InlineKeyboardMarkup>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Message());

        var localizerMock = new Mock<IStringLocalizer<Messages>>();

        var processCallbackQueryHandler =
            new ProcessCallbackQueryHandler(mediatorMock.Object, telegramClientMock.Object, localizerMock.Object);

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
}