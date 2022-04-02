using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Dsl;
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
    private readonly ITelegramBotClient _bot;

    private readonly IPostprocessComposer<Telegram.Bot.Types.CallbackQuery> _callbackQueryComposer;

    private readonly Fixture _fixture;
    private readonly Item _item;
    private readonly IMediator _mediator;
    private readonly Rating _rating;

    private readonly ProcessRatingCommandHandler _sut;
    private readonly User _user;

    public ProcessRatingCommandTests()
    {
        _fixture = new();

        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _user = _fixture.Build<User>()
            .Create();

        _rating = _fixture.Build<Rating>()
            .With(r => r.UserId, _user.Id)
            .Create();

        var ratings = new Collection<Rating>
        {
            _rating
        };

        _item = _fixture.Build<Item>()
            .With(i => i.Ratings, ratings)
            .Create();

        _callbackQueryComposer = _fixture.Build<Telegram.Bot.Types.CallbackQuery>()
            .With(cq => cq.From, _user);

        _mediator = Substitute.For<IMediator>();

        _mediator.Send(Arg.Is(new GetItem(_item.Id)), Arg.Any<CancellationToken>())
            .Returns(_item);

        _bot = Substitute.For<ITelegramBotClient>();

        var localizer = Substitute.For<IStringLocalizer<Messages>>();

        _sut = new(_mediator, _bot, localizer);
    }

    [Fact]
    public async Task RegularMessageCallbackQuery_SetItemRating_Works()
    {
        // Arrange

        var newRatingValue = _fixture.Create<int>();

        var callbackQuery = _callbackQueryComposer
            .With(cq => cq.Data, $"{_item.Id}|r|{newRatingValue}")
            .Without(cq => cq.InlineMessageId)
            .Create();

        // Act

        await _sut.Handle(new(new(callbackQuery)), CancellationToken.None);

        // Assert

        await _mediator.Received().Send(Arg.Is(new SetItemRating(_user.Id, newRatingValue, _item.Id)), Arg.Any<CancellationToken>());
        await _mediator.Received().Send(Arg.Is(new GetItem(_item.Id)), Arg.Any<CancellationToken>());
        await _mediator.Received().Send(Arg.Is(new GetItemMessageText(_item)), Arg.Any<CancellationToken>());
        await _mediator.Received().Send(Arg.Is(new GetRatingsMarkup(_item.Id)), Arg.Any<CancellationToken>());
        await _bot.Received().MakeRequestAsync(Arg.Any<AnswerCallbackQueryRequest>(), Arg.Any<CancellationToken>());
        await _bot.Received().MakeRequestAsync(Arg.Any<EditMessageTextRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RegularMessageCallbackQuery_RefreshRatings_Works()
    {
        // Arrange

        var callbackQuery = _callbackQueryComposer
            .With(cq => cq.Data, $"{_item.Id}|r|null")
            .Without(cq => cq.InlineMessageId)
            .Create();

        // Act

        await _sut.Handle(new(new(callbackQuery)), CancellationToken.None);

        // Assert

        await _mediator.Received().Send(Arg.Is(new GetItem(_item.Id)), Arg.Any<CancellationToken>());
        await _mediator.Received().Send(Arg.Is(new GetItemMessageText(_item)), Arg.Any<CancellationToken>());
        await _mediator.Received().Send(Arg.Is(new GetRatingsMarkup(_item.Id)), Arg.Any<CancellationToken>());
        await _bot.Received().MakeRequestAsync(Arg.Any<EditMessageTextRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RegularMessageCallbackQuery_SetItemSameRating_Works()
    {
        // Arrange

        var callbackQuery = _callbackQueryComposer
            .With(cq => cq.Data, $"{_item.Id}|r|{_rating.Value}")
            .Without(cq => cq.InlineMessageId)
            .Create();

        // Act

        await _sut.Handle(new(new(callbackQuery)), CancellationToken.None);

        // Assert

        await _mediator.Received().Send(Arg.Is(new GetItem(_item.Id)), Arg.Any<CancellationToken>());
        await _bot.Received().MakeRequestAsync(Arg.Any<AnswerCallbackQueryRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task InlineMessageCallbackQuery_SetItemRating_Works()
    {
        // Arrange

        var ratingValue = _fixture.Create<int>();

        var callbackQuery = _callbackQueryComposer
            .With(cq => cq.Data, $"{_item.Id}|r|{ratingValue}")
            .Without(cq => cq.Message)
            .Create();

        // Act

        await _sut.Handle(new(new(callbackQuery)), CancellationToken.None);

        // Assert

        await _mediator.Received().Send(Arg.Is(new SetItemRating(_user.Id, ratingValue, _item.Id)), Arg.Any<CancellationToken>());
        await _mediator.Received().Send(Arg.Is(new GetItem(_item.Id)), Arg.Any<CancellationToken>());
        await _mediator.Received().Send(Arg.Is(new GetItemMessageText(_item)), Arg.Any<CancellationToken>());
        await _mediator.Received().Send(Arg.Is(new GetRatingsMarkup(_item.Id)), Arg.Any<CancellationToken>());
        await _bot.Received().MakeRequestAsync(Arg.Any<EditInlineMessageTextRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task InlineMessageCallbackQuery_Refresh_Works()
    {
        // Arrange

        var callbackQuery = _callbackQueryComposer
            .With(cq => cq.Data, $"{_item.Id}|r|null")
            .Without(cq => cq.Message)
            .Create();

        // Act

        await _sut.Handle(new(new(callbackQuery)), CancellationToken.None);

        // Assert

        await _mediator.Received().Send(Arg.Is(new GetItem(_item.Id)), Arg.Any<CancellationToken>());
        await _mediator.Received().Send(Arg.Is(new GetItemMessageText(_item)), Arg.Any<CancellationToken>());
        await _mediator.Received().Send(Arg.Is(new GetRatingsMarkup(_item.Id)), Arg.Any<CancellationToken>());
        await _bot.Received().MakeRequestAsync(Arg.Any<EditInlineMessageTextRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task InlineMessageCallbackQuery_SetItemSameRating_Works()
    {
        // Arrange

        var callbackQuery = _callbackQueryComposer
            .With(cq => cq.Data, $"{_item.Id}|r|{_rating.Value}")
            .Without(cq => cq.Message)
            .Create();

        // Act

        await _sut.Handle(new(new(callbackQuery)), CancellationToken.None);

        // Assert

        await _mediator.Received().Send(Arg.Is(new GetItem(_item.Id)), Arg.Any<CancellationToken>());
        await _bot.Received().MakeRequestAsync(Arg.Any<AnswerCallbackQueryRequest>(), Arg.Any<CancellationToken>());
    }
}