using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Dsl;
using Bot.Handlers.CallbackQuery;
using Bot.Requests.InlineKeyboardMarkup;
using Bot.Requests.Message;
using Bot.Resources;
using Core.Requests.Item;
using MediatR;
using Microsoft.Extensions.Localization;
using NSubstitute;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Xunit;

namespace Bot.Tests.Handlers.CallbackQuery;

public class ProcessManufacturerCommandTests
{
    private readonly ITelegramBotClient _bot;

    private readonly IPostprocessComposer<Telegram.Bot.Types.CallbackQuery> _callbackQueryComposer;
    private readonly int _itemId;
    private readonly int? _manufacturerId;
    private readonly IMediator _mediator;
    private readonly int _page;

    private readonly ProcessManufacturerCommandHandler _sut;

    public ProcessManufacturerCommandTests()
    {
        var fixture = new Fixture();

        fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _callbackQueryComposer = fixture.Build<Telegram.Bot.Types.CallbackQuery>()
            .Without(cq => cq.InlineMessageId);

        _itemId = fixture.Create<int>();
        _manufacturerId = fixture.Create<int?>();
        _page = fixture.Create<int>();

        _mediator = Substitute.For<IMediator>();
        _bot = Substitute.For<ITelegramBotClient>();

        var localizer = Substitute.For<IStringLocalizer<Messages>>();

        _sut = new(_mediator, _bot, localizer);
    }

    [Fact]
    public async Task RegularMessageCallbackQuery_SetItemManufacturer_Works()
    {
        // Arrange

        var callbackQuery = _callbackQueryComposer
            .With(cq => cq.Data, $"{_itemId}|m|{_page}|{_manufacturerId}")
            .Create();

        // Act

        await _sut.Handle(new(new(callbackQuery)), CancellationToken.None);

        // Assert

        await _mediator.Received().Send(Arg.Is(new SetItemManufacturer(_itemId, _manufacturerId)));
        await _mediator.Received().Send(Arg.Is(new GetPlacesMarkup(_itemId)));
        await _bot.Received().MakeRequestAsync(Arg.Any<EditMessageTextRequest>());
    }

    [Theory]
    [InlineData("{0}|m|{1}|0")]
    [InlineData("{0}|m|{1}|-1")]
    public async Task RegularMessageCallbackQuery_RefreshManufacturers_Works(string dataFormat)
    {
        // Arrange

        var data = string.Format(dataFormat, _itemId, _page);

        var callbackQuery = _callbackQueryComposer
            .With(cq => cq.Data, data)
            .Create();

        // Act

        await _sut.Handle(new(new(callbackQuery)), CancellationToken.None);

        // Assert

        await _mediator.Received().Send(Arg.Is(new GetManufacturersMarkup(_itemId)));
        await _mediator.Received().Send(Arg.Any<EditMessageReplyMarkup>());
    }
}