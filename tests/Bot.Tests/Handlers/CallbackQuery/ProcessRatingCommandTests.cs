using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Dsl;
using Bot.Handlers.CallbackQuery;
using Bot.Requests.InlineKeyboardMarkup;
using Bot.Requests.Message.Item;
using Bot.Resources;
using Core.Models;
using Core.Requests.Item;
using Core.Requests.Rating;
using MediatR;
using Microsoft.Extensions.Localization;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Requests;
using Xunit;
using User = Telegram.Bot.Types.User;

namespace Bot.Tests.Handlers.CallbackQuery;

public class ProcessRatingCommandTests
{
    private readonly ITelegramBotClient _bot;

    private readonly IPostprocessComposer<Telegram.Bot.Types.CallbackQuery> _callbackQueryComposer;

    private readonly IFixture _fixture;
    private readonly Item _item;
    private readonly IMediator _mediator;
    private readonly Rating _rating;

    private readonly ProcessRatingCommandHandler _sut;
    private readonly User _user;

    public ProcessRatingCommandTests()
    {
        _fixture = new Fixture();

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

        _mediator.Send(Arg.Is(new GetItem(_item.Id)))
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

        await _mediator.Received().Send(Arg.Is(new GetRating(_item.Id, _user.Id)));
        await _mediator.Received().Send(Arg.Is(new SetItemRating(_item.Id, _user.Id, newRatingValue)));
        await _mediator.Received().Send(Arg.Is(new GetItem(_item.Id)));
        await _mediator.Received().Send(Arg.Is(new GetItemMessageText(_item)));
        await _mediator.Received().Send(Arg.Is(new GetRatingsMarkup(_item.Id)));
        await _bot.Received().MakeRequestAsync(Arg.Any<AnswerCallbackQueryRequest>());
        await _bot.Received().MakeRequestAsync(Arg.Any<EditMessageTextRequest>());
    }

    [Theory]
    [InlineData("{0}|r|null")]
    [InlineData("{0}|r|0")]
    public async Task RegularMessageCallbackQuery_RefreshRatings_Works(string dataFormat)
    {
        // Arrange

        var data = string.Format(dataFormat, _item.Id);

        var callbackQuery = _callbackQueryComposer
            .With(cq => cq.Data, data)
            .Without(cq => cq.InlineMessageId)
            .Create();

        // Act

        await _sut.Handle(new(new(callbackQuery)), CancellationToken.None);

        // Assert

        await _mediator.Received().Send(Arg.Is(new GetItem(_item.Id)));
        await _mediator.Received().Send(Arg.Is(new GetItemMessageText(_item)));
        await _mediator.Received().Send(Arg.Is(new GetRatingsMarkup(_item.Id)));
        await _bot.Received().MakeRequestAsync(Arg.Any<EditMessageTextRequest>());
    }

    [Fact]
    public async Task RegularMessageCallbackQuery_SetItemSameRating_Works()
    {
        // Arrange

        _mediator.Send(Arg.Is(new GetRating(_item.Id, _user.Id))).Returns(_rating);

        var callbackQuery = _callbackQueryComposer
            .With(cq => cq.Data, $"{_item.Id}|r|{_rating.Value}")
            .Without(cq => cq.InlineMessageId)
            .Create();

        // Act

        await _sut.Handle(new(new(callbackQuery)), CancellationToken.None);

        // Assert

        await _mediator.Received().Send(Arg.Is(new GetRating(_item.Id, _user.Id)));
        await _mediator.Received().Send(Arg.Is(new GetItem(_item.Id)));
        await _bot.Received().MakeRequestAsync(Arg.Any<AnswerCallbackQueryRequest>());
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

        await _mediator.Received().Send(Arg.Is(new GetRating(_item.Id, _user.Id)));
        await _mediator.Received().Send(Arg.Is(new SetItemRating(_item.Id, _user.Id, ratingValue)));
        await _mediator.Received().Send(Arg.Is(new GetItem(_item.Id)));
        await _mediator.Received().Send(Arg.Is(new GetItemMessageText(_item)));
        await _mediator.Received().Send(Arg.Is(new GetRatingsMarkup(_item.Id)));
        await _bot.Received().MakeRequestAsync(Arg.Any<EditInlineMessageTextRequest>());
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

        await _mediator.Received().Send(Arg.Is(new GetItem(_item.Id)));
        await _mediator.Received().Send(Arg.Is(new GetItemMessageText(_item)));
        await _mediator.Received().Send(Arg.Is(new GetRatingsMarkup(_item.Id)));
        await _bot.Received().MakeRequestAsync(Arg.Any<EditInlineMessageTextRequest>());
    }

    [Fact]
    public async Task InlineMessageCallbackQuery_SetItemSameRating_Works()
    {
        // Arrange

        _mediator.Send(Arg.Is(new GetRating(_item.Id, _user.Id))).Returns(_rating);

        var callbackQuery = _callbackQueryComposer
            .With(cq => cq.Data, $"{_item.Id}|r|{_rating.Value}")
            .Without(cq => cq.Message)
            .Create();

        // Act

        await _sut.Handle(new(new(callbackQuery)), CancellationToken.None);

        // Assert

        await _mediator.Received().Send(Arg.Is(new GetRating(_item.Id, _user.Id)));
        await _mediator.Received().Send(Arg.Is(new GetItem(_item.Id)));
        await _bot.Received().MakeRequestAsync(Arg.Any<AnswerCallbackQueryRequest>());
    }

    [Fact]
    public async Task InlineMessageCallbackQuery_SetItemSameRating_SameMessageText_Works()
    {
        // Arrange

        _mediator.Send(Arg.Is(new GetRating(_item.Id, _user.Id))).Returns(_rating);

        _bot.MakeRequestAsync(Arg.Any<EditInlineMessageTextRequest>()).Throws(new ApiRequestException("test"));

        var callbackQuery = _callbackQueryComposer
            .With(cq => cq.Data, $"{_item.Id}|r|{_rating.Value}")
            .Without(cq => cq.Message)
            .Create();

        // Act

        await _sut.Handle(new(new(callbackQuery)), CancellationToken.None);

        // Assert

        await _mediator.Received().Send(Arg.Is(new GetRating(_item.Id, _user.Id)));
        await _mediator.Received().Send(Arg.Is(new GetItem(_item.Id)));
        await _bot.Received(2).MakeRequestAsync(Arg.Any<AnswerCallbackQueryRequest>());
    }
}