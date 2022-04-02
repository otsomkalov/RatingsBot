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
using Telegram.Bot.Types.ReplyMarkups;
using Xunit;

namespace Bot.Tests.Handlers.Message;

public class EditMessageReplyMarkupTests
{
    private readonly ITelegramBotClient _bot;
    private readonly Fixture _fixture;

    private readonly EditMessageReplyMarkupHandler _sut;

    public EditMessageReplyMarkupTests()
    {
        _fixture = new();

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

        var cqd = new CallbackQueryData(callbackQuery);

        var request = new EditMessageReplyMarkup(cqd, _fixture.Create<InlineKeyboardMarkup>());

        // Act

        await _sut.Handle(request, CancellationToken.None);

        // Assert

        await _bot.Received().MakeRequestAsync(Arg.Any<EditMessageReplyMarkupRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Call_WithException_Works()
    {
        // Arrange

        _bot.MakeRequestAsync(Arg.Any<EditMessageReplyMarkupRequest>(), Arg.Any<CancellationToken>())
            .Throws(new ApiRequestException("test"));

        var callbackQuery = _fixture.Build<Telegram.Bot.Types.CallbackQuery>()
            .With(cq => cq.Data, "1|r|1")
            .Create();

        var cqd = new CallbackQueryData(callbackQuery);

        var request = new EditMessageReplyMarkup(cqd, _fixture.Create<InlineKeyboardMarkup>());

        // Act

        await _sut.Handle(request, CancellationToken.None);

        // Assert

        await _bot.Received().MakeRequestAsync(Arg.Any<EditMessageReplyMarkupRequest>(), Arg.Any<CancellationToken>());
        await _bot.Received().MakeRequestAsync(Arg.Any<AnswerCallbackQueryRequest>(), Arg.Any<CancellationToken>());
    }
}