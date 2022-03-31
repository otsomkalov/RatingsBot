using System.Threading;
using System.Threading.Tasks;
using Bot.Commands.Category;
using Bot.Commands.Message;
using Bot.Handlers.Message;
using Bot.Resources;
using Core.Commands.Item;
using Core.Models;
using MediatR;
using Microsoft.Extensions.Localization;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Xunit;

namespace Bot.Tests;

public class ProcessNewMessageTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly Mock<IStringLocalizer<Messages>> _localizer;
    private readonly Mock<ITelegramBotClient> _telegramBotClient;

    public ProcessNewMessageTests()
    {
        _mediator = new();
        _localizer = new();
        _telegramBotClient = new();
    }

    [Fact]
    public async Task MessageFromBot_Works()
    {
        // Arrange

        var handler = new ProcessNewMessageHandler(_localizer.Object, _mediator.Object, _telegramBotClient.Object);

        var message = new Message
        {
            From = new()
            {
                IsBot = true,
            },
            Text = "test"
        };

        var request = new ProcessNewMessage(message);

        // Act

        await handler.Handle(request, CancellationToken.None);

        // Assert
    }

    [Fact]
    public async Task MessageEmptyText_Works()
    {
        // Arrange

        var handler = new ProcessNewMessageHandler(_localizer.Object, _mediator.Object, _telegramBotClient.Object);

        var message = new Message
        {
            From = new(),
            Text = string.Empty
        };

        var request = new ProcessNewMessage(message);

        // Act

        await handler.Handle(request, CancellationToken.None);

        // Assert
    }

    [Theory]
    [InlineData(Constants.Commands.Start)]
    [InlineData(Constants.Commands.NewCategory)]
    [InlineData(Constants.Commands.NewItem)]
    [InlineData(Constants.Commands.NewManufacturer)]
    [InlineData(Constants.Commands.NewPlace)]
    public async Task MessageTextCommands_ExecutesCommands(string command)
    {
        // Arrange

        var handler = new ProcessNewMessageHandler(_localizer.Object, _mediator.Object, _telegramBotClient.Object);

        var message = new Message
        {
            From = new(),
            Text = command
        };

        var request = new ProcessNewMessage(message);

        // Act

        await handler.Handle(request, CancellationToken.None);

        // Assert

        _mediator.Verify(m => m.Send(It.IsAny<IRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ReplyToMessageNewItemCommand_ExecutesCommand()
    {
        // Arrange

        const string command = "command";

        _localizer.SetupGet(m => m[nameof(Messages.NewItemCommand)])
            .Returns(new LocalizedString(nameof(Messages.NewItemCommand), command));

        _mediator.Setup(m => m.Send(It.IsAny<CreateItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Item
            {
                Id = 1
            });

        var handler = new ProcessNewMessageHandler(_localizer.Object, _mediator.Object, _telegramBotClient.Object);

        var message = new Message
        {
            From = new(),
            Text = "test",
            ReplyToMessage = new()
            {
                Text = command
            }
        };

        var request = new ProcessNewMessage(message);

        // Act

        await handler.Handle(request, CancellationToken.None);

        // Assert

        _mediator.Verify(m => m.Send(It.IsAny<GetCategoriesMarkup>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(nameof(Messages.NewManufacturerCommand))]
    [InlineData(nameof(Messages.NewCategoryCommand))]
    [InlineData(nameof(Messages.NewPlaceCommand))]
    public async Task ReplyToMessageCommands_ExecutesCommands(string messageCommand)
    {
        // Arrange

        const string command = "command";

        _localizer.SetupGet(m => m[messageCommand])
            .Returns(new LocalizedString(messageCommand, command));

        var handler = new ProcessNewMessageHandler(_localizer.Object, _mediator.Object, _telegramBotClient.Object);

        var message = new Message
        {
            From = new(),
            Text = "test",
            ReplyToMessage = new()
            {
                Text = command
            }
        };

        var request = new ProcessNewMessage(message);

        // Act

        await handler.Handle(request, CancellationToken.None);

        // Assert

        _mediator.Verify(m => m.Send(It.IsAny<IRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}