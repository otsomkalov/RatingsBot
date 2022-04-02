using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Bot.Handlers.CallbackQuery;
using Bot.Requests.Item;
using Bot.Requests.Rating;
using Bot.Resources;
using Core.Models;
using Core.Requests.Item;
using MediatR;
using Microsoft.Extensions.Localization;
using NSubstitute;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Xunit;
using User = Telegram.Bot.Types.User;

namespace Bot.Tests.Handlers.CallbackQuery;

public class ProcessRatingCommandTests
{
    private readonly IMediator _mediator;
    private readonly ITelegramBotClient _bot;

    private readonly Fixture _fixture;

    private readonly ProcessRatingCommandHandler _sut;

    public ProcessRatingCommandTests()
    {
        _fixture = new();

        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _mediator = Substitute.For<IMediator>();

        _bot = Substitute.For<ITelegramBotClient>();

        var localizer = Substitute.For<IStringLocalizer<Messages>>();

        _sut = new(_mediator, _bot, localizer);
    }

    [Fact]
    public async Task RegularMessageCallbackQuery_SetItemRating_Works()
    {
        // Arrange

        var user = _fixture.Build<User>()
            .Create();

        var rating = _fixture.Build<Rating>()
            .With(r => r.UserId, user.Id)
            .Create();

        var ratings = new Collection<Rating>
        {
            rating
        };

        var item = _fixture.Build<Item>()
            .With(i => i.Ratings, ratings)
            .Create();

        _mediator.Send(Arg.Is(new GetItem(item.Id)), Arg.Any<CancellationToken>())
            .Returns(item);

        var callbackQuery = _fixture.Build<Telegram.Bot.Types.CallbackQuery>()
            .With(cq => cq.Data, $"{item.Id}|r|{rating.Id}")
            .With(cq => cq.From, user)
            .Without(cq => cq.InlineMessageId)
            .Create();

        // Act

        await _sut.Handle(new(new(callbackQuery)), CancellationToken.None);

        // Assert

        await _mediator.Received().Send(Arg.Is(new SetItemRating(user.Id, rating.Id, item.Id)), Arg.Any<CancellationToken>());
        await _mediator.Received().Send(Arg.Is(new GetItem(item.Id)), Arg.Any<CancellationToken>());
        await _mediator.Received().Send(Arg.Is(new GetItemMessageText(item)), Arg.Any<CancellationToken>());
        await _mediator.Received().Send(Arg.Is(new GetRatingsMarkup(item.Id)), Arg.Any<CancellationToken>());
        await _bot.Received().MakeRequestAsync(Arg.Any<AnswerCallbackQueryRequest>(), Arg.Any<CancellationToken>());
        await _bot.Received().MakeRequestAsync(Arg.Any<EditMessageTextRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RegularMessageCallbackQuery_RefreshRatings_Works()
    {
        // Arrange

        var user = _fixture.Build<User>()
            .Create();

        var rating = _fixture.Build<Rating>()
            .With(r => r.UserId, user.Id)
            .Create();

        var ratings = new Collection<Rating>
        {
            rating
        };

        var item = _fixture.Build<Item>()
            .With(i => i.Ratings, ratings)
            .Create();

        _mediator.Send(Arg.Is(new GetItem(item.Id)), Arg.Any<CancellationToken>())
            .Returns(item);

        var callbackQuery = _fixture.Build<Telegram.Bot.Types.CallbackQuery>()
            .With(cq => cq.Data, $"{item.Id}|r|null")
            .With(cq => cq.From, user)
            .Without(cq => cq.InlineMessageId)
            .Create();

        // Act

        await _sut.Handle(new(new(callbackQuery)), CancellationToken.None);

        // Assert

        await _mediator.Received().Send(Arg.Is(new GetItem(item.Id)), Arg.Any<CancellationToken>());
        await _mediator.Received().Send(Arg.Is(new GetItemMessageText(item)), Arg.Any<CancellationToken>());
        await _mediator.Received().Send(Arg.Is(new GetRatingsMarkup(item.Id)), Arg.Any<CancellationToken>());
        await _bot.Received().MakeRequestAsync(Arg.Any<EditMessageTextRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RegularMessageCallbackQuery_SetItemSameRating_Works()
    {
        // Arrange

        var ratingValue = _fixture.Create<int>();

        var user = _fixture.Build<User>()
            .Create();

        var rating = _fixture.Build<Rating>()
            .With(r => r.UserId, user.Id)
            .With(r => r.Value, ratingValue)
            .Create();

        var ratings = new Collection<Rating>
        {
            rating
        };

        var item = _fixture.Build<Item>()
            .With(i => i.Ratings, ratings)
            .Create();

        _mediator.Send(Arg.Is(new GetItem(item.Id)), Arg.Any<CancellationToken>())
            .Returns(item);

        var callbackQuery = _fixture.Build<Telegram.Bot.Types.CallbackQuery>()
            .With(cq => cq.Data, $"{item.Id}|r|{ratingValue}")
            .With(cq => cq.From, user)
            .Without(cq => cq.InlineMessageId)
            .Create();

        // Act

        await _sut.Handle(new(new(callbackQuery)), CancellationToken.None);

        // Assert

        await _mediator.Received().Send(Arg.Is(new GetItem(item.Id)), Arg.Any<CancellationToken>());
        await _bot.Received().MakeRequestAsync(Arg.Any<AnswerCallbackQueryRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task InlineMessageCallbackQuery_SetItemRating_Works()
    {
        // Arrange

        var ratingValue = 5;

        var user = _fixture.Build<User>()
            .Create();

        var rating = _fixture.Build<Rating>()
            .With(r => r.UserId, user.Id)
            .Create();

        var ratings = new Collection<Rating>
        {
            rating
        };

        var item = _fixture.Build<Item>()
            .With(i => i.Ratings, ratings)
            .Create();

        _mediator.Send(Arg.Is(new GetItem(item.Id)), Arg.Any<CancellationToken>())
            .Returns(item);

        var callbackQuery = _fixture.Build<Telegram.Bot.Types.CallbackQuery>()
            .With(cq => cq.Data, $"{item.Id}|r|{ratingValue}")
            .With(cq => cq.From, user)
            .Without(cq => cq.Message)
            .Create();

        // Act

        await _sut.Handle(new(new(callbackQuery)), CancellationToken.None);

        // Assert

        await _mediator.Received().Send(Arg.Is(new SetItemRating(user.Id, ratingValue, item.Id)), Arg.Any<CancellationToken>());
        await _mediator.Received().Send(Arg.Is(new GetItem(item.Id)), Arg.Any<CancellationToken>());
        await _mediator.Received().Send(Arg.Is(new GetItemMessageText(item)), Arg.Any<CancellationToken>());
        await _mediator.Received().Send(Arg.Is(new GetRatingsMarkup(item.Id)), Arg.Any<CancellationToken>());
        await _bot.Received().MakeRequestAsync(Arg.Any<EditInlineMessageTextRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task InlineMessageCallbackQuery_Refresh_Works()
    {
        // Arrange

        var user = _fixture.Build<User>()
            .Create();

        var rating = _fixture.Build<Rating>()
            .With(r => r.UserId, user.Id)
            .Create();

        var ratings = new Collection<Rating>
        {
            rating
        };

        var item = _fixture.Build<Item>()
            .With(i => i.Ratings, ratings)
            .Create();

        _mediator.Send(Arg.Is(new GetItem(item.Id)), Arg.Any<CancellationToken>())
            .Returns(item);

        var callbackQuery = _fixture.Build<Telegram.Bot.Types.CallbackQuery>()
            .With(cq => cq.Data, $"{item.Id}|r|null")
            .With(cq => cq.From, user)
            .Without(cq => cq.Message)
            .Create();

        // Act

        await _sut.Handle(new(new(callbackQuery)), CancellationToken.None);

        // Assert

        await _mediator.Received().Send(Arg.Is(new GetItem(item.Id)), Arg.Any<CancellationToken>());
        await _mediator.Received().Send(Arg.Is(new GetItemMessageText(item)), Arg.Any<CancellationToken>());
        await _mediator.Received().Send(Arg.Is(new GetRatingsMarkup(item.Id)), Arg.Any<CancellationToken>());
        await _bot.Received().MakeRequestAsync(Arg.Any<EditInlineMessageTextRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task InlineMessageCallbackQuery_SetItemSameRating_Works()
    {
        // Arrange

        var user = _fixture.Build<User>()
            .Create();

        var rating = _fixture.Build<Rating>()
            .With(r => r.UserId, user.Id)
            .Create();

        var ratings = new Collection<Rating>
        {
            rating
        };

        var item = _fixture.Build<Item>()
            .With(i => i.Ratings, ratings)
            .Create();

        _mediator.Send(Arg.Is(new GetItem(item.Id)), Arg.Any<CancellationToken>())
            .Returns(item);

        var callbackQuery = _fixture.Build<Telegram.Bot.Types.CallbackQuery>()
            .With(cq => cq.Data, $"{item.Id}|r|{rating.Value}")
            .With(cq => cq.From, user)
            .Without(cq => cq.Message)
            .Create();

        // Act

        await _sut.Handle(new(new(callbackQuery)), CancellationToken.None);

        // Assert

        await _mediator.Received().Send(Arg.Is(new GetItem(item.Id)), Arg.Any<CancellationToken>());
        await _bot.Received().MakeRequestAsync(Arg.Any<AnswerCallbackQueryRequest>(), Arg.Any<CancellationToken>());
    }
}