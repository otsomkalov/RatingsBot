using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Dsl;
using Bot.Constants;
using Bot.Handlers.CallbackQuery;
using Bot.Requests.InlineKeyboardMarkup;
using Bot.Requests.Message;
using Bot.Requests.Message.Item;
using Core.Requests.Item;
using MediatR;
using NSubstitute;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Xunit;

namespace Bot.Tests.Handlers.CallbackQuery;

public class ProcessPlaceCommandTests
{
    private readonly ITelegramBotClient _bot;
    private readonly IPostprocessComposer<Telegram.Bot.Types.CallbackQuery> _callbackQueryComposer;
    private readonly int _itemId;
    private readonly IMediator _mediator;
    private readonly int _page;
    private readonly int? _placeId;

    private readonly ProcessPlaceCommandHandler _sut;

    public ProcessPlaceCommandTests()
    {
        var fixture = new Fixture();

        fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _callbackQueryComposer = fixture.Build<Telegram.Bot.Types.CallbackQuery>()
            .Without(cq => cq.InlineMessageId);

        _itemId = fixture.Create<int>();
        _placeId = fixture.Create<int?>();
        _page = fixture.Create<int>();

        _mediator = Substitute.For<IMediator>();
        _bot = Substitute.For<ITelegramBotClient>();

        _sut = new(_mediator, _bot);
    }

    [Fact]
    public async Task RegularMessageCallbackQuery_SetItemPlace_Works()
    {
        // Arrange

        var callbackQuery = _callbackQueryComposer
            .With(cq => cq.Data, $"{_itemId}|p|{_page}|{_placeId}")
            .Create();

        // Act

        await _sut.Handle(new(new(callbackQuery)), CancellationToken.None);

        // Assert

        await _mediator.Received().Send(Arg.Is(new SetItemPlace(_itemId, _placeId)));
        await _mediator.Received().Send(Arg.Is(new GetItem(_itemId)));
        await _mediator.Received().Send(Arg.Is(new GetRatingsMarkup(_itemId)));
        await _mediator.Received().Send(Arg.Any<GetItemMessageText>());
        await _bot.Received().MakeRequestAsync(Arg.Any<EditMessageTextRequest>());
    }

    [Theory]
    [InlineData("{0}|p|{1}|0")]
    [InlineData("{0}|p|{1}|-1")]
    public async Task RegularMessageCallbackQuery_RefreshPlaces_Works(string dataFormat)
    {
        // Arrange

        var data = string.Format(dataFormat, _itemId, _page);

        var callbackQuery = _callbackQueryComposer
            .With(cq => cq.Data, data)
            .Create();

        // Act

        await _sut.Handle(new(new(callbackQuery)), CancellationToken.None);

        // Assert

        await _mediator.Received().Send(Arg.Is(new GetInlineKeyboardMarkup(_itemId, ReplyMarkup.Place, _page)));
        await _mediator.Received().Send(Arg.Any<EditMessageReplyMarkup>());
    }
}