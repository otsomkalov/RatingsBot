using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Bot.Constants;
using Bot.Handlers.InlineKeyboardMarkup;
using Bot.Requests.InlineKeyboardMarkup;
using Core.Models;
using Data;
using EntityFrameworkCoreMock.NSubstitute;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace Bot.Tests.Handlers.InlineKeyboardMarkup;

public class GetInlineKeyboardMarkupTests
{
    private const int ButtonsPerPage = 2;
    private const int ButtonsPerRow = 1;

    private readonly IFixture _fixture;
    private readonly int _itemId;

    private readonly IMediator _mediator;

    public GetInlineKeyboardMarkupTests()
    {
        _fixture = new Fixture();

        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _itemId = _fixture.Create<int>();

        _mediator = Substitute.For<IMediator>();

        _mediator.Send(Arg.Any<GetKeyboardSettings>()).Returns((ButtonsPerPage, ButtonsPerRow));
    }

    [Fact]
    public async Task FirstPage_CountLessThanPageSize_NoNextPage_ReturnsCorrectKeyboard()
    {
        // Arrange

        var contextMock = new DbContextMock<AppDbContext>(new DbContextOptions<AppDbContext>());
        var categories = new List<Category>();

        _fixture.AddManyTo(categories, 1);

        contextMock.CreateDbSetMock(m => m.Categories, categories);

        var handler = new GetInlineKeyboardMarkupHandler(contextMock.Object, _mediator);

        var getCategoriesMarkup = new GetInlineKeyboardMarkup(_itemId, ReplyMarkup.Category);

        // Act

        var inlineKeyboardMarkup = await handler.Handle(getCategoriesMarkup, CancellationToken.None);

        // Assert

        const int categoriesRowsCount = 1;
        inlineKeyboardMarkup.InlineKeyboard.Count().Should().Be(categoriesRowsCount + 1);

        foreach (var buttonsRow in inlineKeyboardMarkup.InlineKeyboard.Take(categoriesRowsCount))
        {
            buttonsRow.Count().Should().Be(ButtonsPerRow);

            foreach (var button in buttonsRow)
            {
                button.CallbackData.Should().Contain(ReplyMarkup.Category);
            }
        }

        var lastRowButtons = inlineKeyboardMarkup.InlineKeyboard.Last().ToImmutableArray();

        lastRowButtons.Length.Should().Be(1);
        lastRowButtons[0].Text.Should().Be(ReplyMarkup.RefreshButtonText);
    }

    [Fact]
    public async Task FirstPage_CountEqualsPageSize_NoNextPage_ReturnsCorrectKeyboard()
    {
        // Arrange

        var contextMock = new DbContextMock<AppDbContext>(new DbContextOptions<AppDbContext>());
        var categories = new List<Category>();

        _fixture.AddManyTo(categories, 2);

        contextMock.CreateDbSetMock(m => m.Categories, categories);

        var handler = new GetInlineKeyboardMarkupHandler(contextMock.Object, _mediator);

        var getCategoriesMarkup = new GetInlineKeyboardMarkup(_itemId, ReplyMarkup.Category);

        // Act

        var inlineKeyboardMarkup = await handler.Handle(getCategoriesMarkup, CancellationToken.None);

        // Assert

        const int categoriesRowsCount = 2;
        inlineKeyboardMarkup.InlineKeyboard.Count().Should().Be(categoriesRowsCount + 1);

        foreach (var buttonsRow in inlineKeyboardMarkup.InlineKeyboard.Take(categoriesRowsCount))
        {
            buttonsRow.Count().Should().Be(ButtonsPerRow);

            foreach (var button in buttonsRow)
            {
                button.CallbackData.Should().Contain(ReplyMarkup.Category);
            }
        }

        var lastRowButtons = inlineKeyboardMarkup.InlineKeyboard.Last().ToImmutableArray();

        lastRowButtons.Length.Should().Be(1);
        lastRowButtons[0].Text.Should().Be(ReplyMarkup.RefreshButtonText);
    }

    [Fact]
    public async Task FirstPage_HasNextPage_ReturnsCorrectKeyboard()
    {
        // Arrange

        var contextMock = new DbContextMock<AppDbContext>(new DbContextOptions<AppDbContext>());
        var categories = new List<Category>();

        _fixture.AddManyTo(categories, 3);

        contextMock.CreateDbSetMock(m => m.Categories, categories);

        var handler = new GetInlineKeyboardMarkupHandler(contextMock.Object, _mediator);

        var getCategoriesMarkup = new GetInlineKeyboardMarkup(_itemId, ReplyMarkup.Category);

        // Act

        var inlineKeyboardMarkup = await handler.Handle(getCategoriesMarkup, CancellationToken.None);

        // Assert

        const int categoriesRowsCount = 2;
        inlineKeyboardMarkup.InlineKeyboard.Count().Should().Be(categoriesRowsCount + 1);

        foreach (var buttonsRow in inlineKeyboardMarkup.InlineKeyboard.Take(categoriesRowsCount))
        {
            buttonsRow.Count().Should().Be(ButtonsPerRow);

            foreach (var button in buttonsRow)
            {
                button.CallbackData.Should().Contain(ReplyMarkup.Category);
            }
        }

        var lastRowButtons = inlineKeyboardMarkup.InlineKeyboard.Last().ToImmutableArray();

        lastRowButtons.Length.Should().Be(2);
        lastRowButtons[0].Text.Should().Be(ReplyMarkup.RefreshButtonText);
        lastRowButtons[1].Text.Should().Be(ReplyMarkup.NextPageButtonText);
        lastRowButtons[1].CallbackData.Should().Be($"{_itemId}{ReplyMarkup.Separator}{ReplyMarkup.Category}{ReplyMarkup.Separator}{1}{ReplyMarkup.Separator}{0}");
    }

    [Fact]
    public async Task MiddlePage_HasNextPage_ReturnsCorrectKeyboard()
    {
        // Arrange

        var contextMock = new DbContextMock<AppDbContext>(new DbContextOptions<AppDbContext>());
        var categories = new List<Category>();

        _fixture.AddManyTo(categories, 5);

        contextMock.CreateDbSetMock(m => m.Categories, categories);

        var handler = new GetInlineKeyboardMarkupHandler(contextMock.Object, _mediator);

        var getCategoriesMarkup = new GetInlineKeyboardMarkup(_itemId, ReplyMarkup.Category, 1);

        // Act

        var inlineKeyboardMarkup = await handler.Handle(getCategoriesMarkup, CancellationToken.None);

        // Assert

        const int categoriesRowsCount = 2;
        inlineKeyboardMarkup.InlineKeyboard.Count().Should().Be(categoriesRowsCount + 1);

        foreach (var buttonsRow in inlineKeyboardMarkup.InlineKeyboard.Take(categoriesRowsCount))
        {
            buttonsRow.Count().Should().Be(ButtonsPerRow);

            foreach (var button in buttonsRow)
            {
                button.CallbackData.Should().Contain(ReplyMarkup.Category);
            }
        }

        var lastRowButtons = inlineKeyboardMarkup.InlineKeyboard.Last().ToImmutableArray();

        lastRowButtons.Length.Should().Be(3);

        lastRowButtons[0].Text.Should().Be(ReplyMarkup.PreviousPageButtonText);
        lastRowButtons[0].CallbackData.Should().Be($"{_itemId}{ReplyMarkup.Separator}{ReplyMarkup.Category}{ReplyMarkup.Separator}{0}{ReplyMarkup.Separator}{0}");
        lastRowButtons[1].Text.Should().Be(ReplyMarkup.RefreshButtonText);
        lastRowButtons[2].Text.Should().Be(ReplyMarkup.NextPageButtonText);
        lastRowButtons[2].CallbackData.Should().Be($"{_itemId}{ReplyMarkup.Separator}{ReplyMarkup.Category}{ReplyMarkup.Separator}{2}{ReplyMarkup.Separator}{0}");
    }

    [Fact]
    public async Task LastPage_CountLessPageSize_ReturnsCorrectKeyboard()
    {
        // Arrange

        var contextMock = new DbContextMock<AppDbContext>(new DbContextOptions<AppDbContext>());
        var categories = new List<Category>();

        _fixture.AddManyTo(categories, 3);

        contextMock.CreateDbSetMock(m => m.Categories, categories);

        var handler = new GetInlineKeyboardMarkupHandler(contextMock.Object, _mediator);

        var getCategoriesMarkup = new GetInlineKeyboardMarkup(_itemId, ReplyMarkup.Category, 1);

        // Act

        var inlineKeyboardMarkup = await handler.Handle(getCategoriesMarkup, CancellationToken.None);

        // Assert

        const int categoriesRowsCount = 1;
        inlineKeyboardMarkup.InlineKeyboard.Count().Should().Be(categoriesRowsCount + 1);

        foreach (var buttonsRow in inlineKeyboardMarkup.InlineKeyboard.Take(categoriesRowsCount))
        {
            buttonsRow.Count().Should().Be(ButtonsPerRow);

            foreach (var button in buttonsRow)
            {
                button.CallbackData.Should().Contain(ReplyMarkup.Category);
            }
        }

        var lastRowButtons = inlineKeyboardMarkup.InlineKeyboard.Last().ToImmutableArray();

        lastRowButtons.Length.Should().Be(2);

        lastRowButtons[0].Text.Should().Be(ReplyMarkup.PreviousPageButtonText);
        lastRowButtons[0].CallbackData.Should().Be($"{_itemId}{ReplyMarkup.Separator}{ReplyMarkup.Category}{ReplyMarkup.Separator}{0}{ReplyMarkup.Separator}{0}");
        lastRowButtons[1].Text.Should().Be(ReplyMarkup.RefreshButtonText);
    }

    [Fact]
    public async Task LastPage_CountEqualsPageSize_ReturnsCorrectKeyboard()
    {
        // Arrange

        var contextMock = new DbContextMock<AppDbContext>(new DbContextOptions<AppDbContext>());
        var categories = new List<Category>();

        _fixture.AddManyTo(categories, 4);

        contextMock.CreateDbSetMock(m => m.Categories, categories);

        var handler = new GetInlineKeyboardMarkupHandler(contextMock.Object, _mediator);

        var getCategoriesMarkup = new GetInlineKeyboardMarkup(_itemId, ReplyMarkup.Category, 1);

        // Act

        var inlineKeyboardMarkup = await handler.Handle(getCategoriesMarkup, CancellationToken.None);

        // Assert

        var categoriesRowsCount = 2;
        inlineKeyboardMarkup.InlineKeyboard.Count().Should().Be(categoriesRowsCount + 1);

        foreach (var buttonsRow in inlineKeyboardMarkup.InlineKeyboard.Take(categoriesRowsCount))
        {
            buttonsRow.Count().Should().Be(ButtonsPerRow);

            foreach (var button in buttonsRow)
            {
                button.CallbackData.Should().Contain(ReplyMarkup.Category);
            }
        }

        var lastRowButtons = inlineKeyboardMarkup.InlineKeyboard.Last().ToImmutableArray();

        lastRowButtons.Length.Should().Be(2);

        lastRowButtons[0].Text.Should().Be(ReplyMarkup.PreviousPageButtonText);
        lastRowButtons[0].CallbackData.Should().Be($"{_itemId}{ReplyMarkup.Separator}{ReplyMarkup.Category}{ReplyMarkup.Separator}{0}{ReplyMarkup.Separator}{0}");
        lastRowButtons[1].Text.Should().Be(ReplyMarkup.RefreshButtonText);
    }

    [Fact]
    public async Task FirstPageOfPlaces_CountLessThanPageSize_NoNextPage_ReturnsCorrectKeyboard()
    {
        // Arrange

        var contextMock = new DbContextMock<AppDbContext>(new DbContextOptions<AppDbContext>());
        var places = new List<Place>();

        _fixture.AddManyTo(places, 1);

        contextMock.CreateDbSetMock(m => m.Places, places);

        var handler = new GetInlineKeyboardMarkupHandler(contextMock.Object, _mediator);

        var getPlacesMarkup = new GetInlineKeyboardMarkup(_itemId, ReplyMarkup.Place);

        // Act

        var inlineKeyboardMarkup = await handler.Handle(getPlacesMarkup, CancellationToken.None);

        // Assert

        const int placesRowsCount = 1;
        var inlineKeyboard = inlineKeyboardMarkup.InlineKeyboard.ToImmutableArray();

        inlineKeyboard.Length.Should().Be(placesRowsCount + 2);

        foreach (var buttonsRow in inlineKeyboard.Take(placesRowsCount))
        {
            buttonsRow.Count().Should().Be(ButtonsPerRow);

            foreach (var button in buttonsRow)
            {
                button.CallbackData.Should().Contain(ReplyMarkup.Place);
            }
        }

        var noneRowButtons = inlineKeyboard[^2].ToImmutableArray();

        noneRowButtons.Length.Should().Be(1);
        noneRowButtons[0].Text.Should().Be(ReplyMarkup.NoneButtonText);
        noneRowButtons[0].CallbackData.Should().EndWith(ReplyMarkup.Separator);

        var lastRowButtons = inlineKeyboard[^1].ToImmutableArray();

        lastRowButtons.Length.Should().Be(1);
        lastRowButtons[0].Text.Should().Be(ReplyMarkup.RefreshButtonText);
        lastRowButtons[0].CallbackData.Should().Contain("0");
    }
}