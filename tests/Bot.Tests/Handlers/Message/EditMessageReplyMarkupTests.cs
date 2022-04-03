using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Bot.Handlers.Message;
using Bot.Models;
using Bot.Requests.Message;
using Bot.Resources;
using Microsoft.Extensions.Localization;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Requests;
using Xunit;

namespace Bot.Tests.Handlers.Message;

public class EditMessageReplyMarkupTests
{
    private readonly ITelegramBotClient _bot;
    private readonly IFixture _fixture;

    private readonly EditMessageReplyMarkupHandler _sut;

    public EditMessageReplyMarkupTests()
    {
        _fixture = new Fixture();

        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _bot = Substitute.For<ITelegramBotClient>();

        var localizer = Substitute.For<IStringLocalizer<Messages>>();

        _sut = new(_bot, localizer);
    }

    [Fact]
    public async Task Call_WithoutException_Works()
    {
        // Arrange

        var callbackQuery = _fixture.Build<Telegram.Bot.Types.CallbackQuery>()
            .With(cq => cq.Data, "1|r|1")
            .Create();

        var request = new EditMessageReplyMarkup(new RatingCallbackQueryData(callbackQuery),
            _fixture.Create<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup>());

        // Act

        await _sut.Handle(request, CancellationToken.None);

        // Assert

        await _bot.Received().MakeRequestAsync(Arg.Any<EditMessageReplyMarkupRequest>());
    }

    [Fact]
    public async Task Call_WithException_Works()
    {
        // Arrange

        _bot.MakeRequestAsync(Arg.Any<EditMessageReplyMarkupRequest>())
            .Throws(new ApiRequestException("test"));

        var callbackQuery = _fixture.Build<Telegram.Bot.Types.CallbackQuery>()
            .With(cq => cq.Data, "1|r|1")
            .Create();

        var request = new EditMessageReplyMarkup(new RatingCallbackQueryData(callbackQuery),
            _fixture.Create<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup>());

        // Act

        await _sut.Handle(request, CancellationToken.None);

        // Assert

        await _bot.Received().MakeRequestAsync(Arg.Any<EditMessageReplyMarkupRequest>());
        await _bot.Received().MakeRequestAsync(Arg.Any<AnswerCallbackQueryRequest>());
    }
}