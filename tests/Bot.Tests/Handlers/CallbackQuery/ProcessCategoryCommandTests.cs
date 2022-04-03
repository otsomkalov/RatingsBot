using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Dsl;
using Bot.Constants;
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

public class ProcessCategoryCommandTests
{
    private readonly ITelegramBotClient _bot;

    private readonly IPostprocessComposer<Telegram.Bot.Types.CallbackQuery> _callbackQueryComposer;
    private readonly int _categoryId;
    private readonly int _itemId;
    private readonly IMediator _mediator;
    private readonly int _page;

    private readonly ProcessCategoryCommandHandler _sut;

    public ProcessCategoryCommandTests()
    {
        var fixture = new Fixture();

        fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _callbackQueryComposer = fixture.Build<Telegram.Bot.Types.CallbackQuery>()
            .Without(cq => cq.InlineMessageId);

        _itemId = fixture.Create<int>();
        _categoryId = fixture.Create<int>();
        _page = fixture.Create<int>();

        _mediator = Substitute.For<IMediator>();
        _bot = Substitute.For<ITelegramBotClient>();

        var lozalizer = Substitute.For<IStringLocalizer<Messages>>();

        _sut = new(_mediator, _bot, lozalizer);
    }

    [Fact]
    public async Task RegularMessageCallbackQuery_SetCategory_Works()
    {
        // Arrange

        var callbackQuery = _callbackQueryComposer
            .With(cq => cq.Data, $"{_itemId}|c|{_page}|{_categoryId}")
            .Create();

        // Act

        await _sut.Handle(new(new(callbackQuery)), CancellationToken.None);

        // Assert

        await _mediator.Received().Send(Arg.Is(new SetItemCategory(_itemId, _categoryId)));
        await _mediator.Received().Send(Arg.Is(new GetInlineKeyboardMarkup(_itemId, ReplyMarkup.Manufacturer)));
        await _bot.Received().MakeRequestAsync(Arg.Any<EditMessageTextRequest>());
    }

    [Theory]
    [InlineData("{0}|c|{1}|null")]
    [InlineData("{0}|c|{1}|0")]
    public async Task RegularMessageCallbackQuery_RefreshCategories_Works(string dataFormat)
    {
        // Arrange

        var data = string.Format(dataFormat, _itemId, _page);

        var callbackQuery = _callbackQueryComposer
            .With(cq => cq.Data, data)
            .Create();

        // Act

        await _sut.Handle(new(new(callbackQuery)), CancellationToken.None);

        // Assert

        await _mediator.Received().Send(Arg.Is(new GetInlineKeyboardMarkup(_itemId, ReplyMarkup.Category, _page)));
        await _mediator.Received().Send(Arg.Any<EditMessageReplyMarkup>());
    }
}