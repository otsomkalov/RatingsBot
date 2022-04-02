using System.Threading;
using System.Threading.Tasks;
using Bot.Commands.CallbackQuery;
using Bot.Commands.Category;
using Bot.Commands.Item;
using Bot.Commands.Manufacturer;
using Bot.Commands.Place;
using Bot.Commands.Rating;
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
    private readonly Mock<IMediator> _mediator;

    public ProcessCallbackQueryTests()
    {
        _mediator = new();
        _telegramClient = new();
        var localizer = new Mock<IStringLocalizer<Messages>>();

        _mediator.Setup(m => m.Send(It.IsAny<GetItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Item
            {
                Id = 1
            });

        _sut = new(_mediator.Object, _telegramClient.Object, localizer.Object);
    }

    [Fact]
    public async Task RegularMessageCallbackQuery_SetCategory_Works()
    {
        // Arrange

        var callbackQuery = new ProcessCallbackQuery(new()
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
        });

        // Act

        await _sut.Handle(callbackQuery, CancellationToken.None);

        // Assert

        _mediator.Verify(m => m.Send(It.IsAny<SetItemCategory>(), It.IsAny<CancellationToken>()));
        _mediator.Verify(m => m.Send(It.IsAny<GetManufacturersMarkup>(), It.IsAny<CancellationToken>()));
        _telegramClient.Verify(m => m.MakeRequestAsync(It.IsAny<EditMessageTextRequest>(), It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task RegularMessageCallbackQuery_RefreshCategories_Works()
    {
        // Arrange

        var callbackQuery = new ProcessCallbackQuery(new()
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
        });

        // Act

        await _sut.Handle(callbackQuery, CancellationToken.None);

        // Assert

        _mediator.Verify(m => m.Send(It.IsAny<GetCategoriesMarkup>(), It.IsAny<CancellationToken>()));
        _telegramClient.Verify(m => m.MakeRequestAsync(It.IsAny<EditMessageReplyMarkupRequest>(), It.IsAny<CancellationToken>()));
    }

    [Theory]
    [InlineData("1|m|1")]
    [InlineData("1|m|null")]
    public async Task RegularMessageCallbackQuery_SetItemManufacturer_Works(string data)
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

        _mediator.Verify(m => m.Send(It.IsAny<SetItemManufacturer>(), It.IsAny<CancellationToken>()));
        _mediator.Verify(m => m.Send(It.IsAny<GetPlacesMarkup>(), It.IsAny<CancellationToken>()));
        _telegramClient.Verify(m => m.MakeRequestAsync(It.IsAny<EditMessageTextRequest>(), It.IsAny<CancellationToken>()));
    }

    [Theory]
    [InlineData("1|m|0")]
    [InlineData("1|m|-1")]
    public async Task RegularMessageCallbackQuery_RefreshManufacturers_Works(string data)
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

        _mediator.Verify(m => m.Send(It.IsAny<GetManufacturersMarkup>(), It.IsAny<CancellationToken>()));
        _telegramClient.Verify(m => m.MakeRequestAsync(It.IsAny<EditMessageReplyMarkupRequest>(), It.IsAny<CancellationToken>()));
    }

    [Theory]
    [InlineData("1|p|1")]
    [InlineData("1|p|null")]
    public async Task RegularMessageCallbackQuery_SetItemPlace_Works(string data)
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

        _mediator.Verify(m => m.Send(It.IsAny<SetItemPlace>(), It.IsAny<CancellationToken>()));
        _mediator.Verify(m => m.Send(It.IsAny<GetItem>(), It.IsAny<CancellationToken>()));
        _mediator.Verify(m => m.Send(It.IsAny<GetRatingsMarkup>(), It.IsAny<CancellationToken>()));
        _mediator.Verify(m => m.Send(It.IsAny<GetItemMessageText>(), It.IsAny<CancellationToken>()));
        _telegramClient.Verify(m => m.MakeRequestAsync(It.IsAny<EditMessageTextRequest>(), It.IsAny<CancellationToken>()));
    }

    [Theory]
    [InlineData("1|p|0")]
    [InlineData("1|p|-1")]
    public async Task RegularMessageCallbackQuery_RefreshPlaces_Works(string data)
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

        _mediator.Verify(m => m.Send(It.IsAny<GetPlacesMarkup>(), It.IsAny<CancellationToken>()));
        _telegramClient.Verify(m => m.MakeRequestAsync(It.IsAny<EditMessageReplyMarkupRequest>(), It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task RegularMessageCallbackQuery_SetItemRating_Works()
    {
        // Arrange

        var callbackQuery = new ProcessCallbackQuery(new()
        {
            Data = "1|r|5",
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

        _mediator.Verify(m => m.Send(It.IsAny<SetItemRating>(), It.IsAny<CancellationToken>()));
        _mediator.Verify(m => m.Send(It.IsAny<GetItem>(), It.IsAny<CancellationToken>()));
        _mediator.Verify(m => m.Send(It.IsAny<GetItemMessageText>(), It.IsAny<CancellationToken>()));
        _mediator.Verify(m => m.Send(It.IsAny<GetRatingsMarkup>(), It.IsAny<CancellationToken>()));
        _telegramClient.Verify(m => m.MakeRequestAsync(It.IsAny<AnswerCallbackQueryRequest>(), It.IsAny<CancellationToken>()));
        _telegramClient.Verify(m => m.MakeRequestAsync(It.IsAny<EditMessageTextRequest>(), It.IsAny<CancellationToken>()));
    }

    [Theory]
    [InlineData("1|r|null")]
    [InlineData("1|r|-1")]
    public async Task RegularMessageCallbackQuery_RefreshRatings_Works(string data)
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

        _mediator.Verify(m => m.Send(It.IsAny<GetItem>(), It.IsAny<CancellationToken>()));
        _mediator.Verify(m => m.Send(It.IsAny<GetItemMessageText>(), It.IsAny<CancellationToken>()));
        _mediator.Verify(m => m.Send(It.IsAny<GetRatingsMarkup>(), It.IsAny<CancellationToken>()));
        _telegramClient.Verify(m => m.MakeRequestAsync(It.IsAny<EditMessageTextRequest>(), It.IsAny<CancellationToken>()));
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
    [InlineData("1|r|0")]
    [InlineData("1|p|-1")]
    [InlineData("1|p|0")]
    [InlineData("1|c|null")]
    [InlineData("1|c|0")]
    [InlineData("1|m|-1")]
    [InlineData("1|m|0")]
    public async Task InlineMessageCallbackQuery_Refresh_Works(string data)
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