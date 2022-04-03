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

public class GetCategoriesMarkupTests
{
    private const int ButtonsPerPage = 2;
    private const int ButtonsPerRow = 1;

    private readonly IFixture _fixture;
    private readonly int _itemId;

    private readonly IMediator _mediator;

    public GetCategoriesMarkupTests()
    {
        _fixture = new Fixture();

        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _itemId = _fixture.Create<int>();

        _mediator = Substitute.For<IMediator>();

        _mediator.Send(Arg.Any<GetKeyboardSettings>()).Returns((ButtonsPerPage, ColumnsCount: ButtonsPerRow));
    }

    [Fact]
    public async Task FirstPage_CountLessThanPageSize_NoNextPage_ReturnsCorrectKeyboard()
    {
        // Arrange

        var contextMock = new DbContextMock<AppDbContext>(new DbContextOptions<AppDbContext>());
        var categories = new List<Category>();

        _fixture.AddManyTo(categories, 1);

        contextMock.CreateDbSetMock(m => m.Categories, categories);

        var handler = new GetCategoriesMarkupHandler(contextMock.Object, _mediator);

        var getCategoriesMarkup = new GetCategoriesMarkup(_itemId);

        // Act

        var inlineKeyboardMarkup = await handler.Handle(getCategoriesMarkup, CancellationToken.None);

        // Assert

        const int categoriesRowsCount = 1;
        inlineKeyboardMarkup.InlineKeyboard.Count().Should().Be(categoriesRowsCount + 1);

        foreach (var buttonsRow in inlineKeyboardMarkup.InlineKeyboard.Take(categoriesRowsCount))
        {
            buttonsRow.Count().Should().Be(ButtonsPerRow);
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

        var handler = new GetCategoriesMarkupHandler(contextMock.Object, _mediator);

        var getCategoriesMarkup = new GetCategoriesMarkup(_itemId);

        // Act

        var inlineKeyboardMarkup = await handler.Handle(getCategoriesMarkup, CancellationToken.None);

        // Assert

        const int categoriesRowsCount = 2;
        inlineKeyboardMarkup.InlineKeyboard.Count().Should().Be(categoriesRowsCount + 1);

        foreach (var buttonsRow in inlineKeyboardMarkup.InlineKeyboard.Take(categoriesRowsCount))
        {
            buttonsRow.Count().Should().Be(ButtonsPerRow);
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

        var handler = new GetCategoriesMarkupHandler(contextMock.Object, _mediator);

        var getCategoriesMarkup = new GetCategoriesMarkup(_itemId);

        // Act

        var inlineKeyboardMarkup = await handler.Handle(getCategoriesMarkup, CancellationToken.None);

        // Assert

        const int categoriesRowsCount = 2;
        inlineKeyboardMarkup.InlineKeyboard.Count().Should().Be(categoriesRowsCount + 1);

        foreach (var buttonsRow in inlineKeyboardMarkup.InlineKeyboard.Take(categoriesRowsCount))
        {
            buttonsRow.Count().Should().Be(ButtonsPerRow);
        }

        var lastRowButtons = inlineKeyboardMarkup.InlineKeyboard.Last().ToImmutableArray();

        lastRowButtons.Length.Should().Be(2);
        lastRowButtons[0].Text.Should().Be(ReplyMarkup.RefreshButtonText);
        lastRowButtons[1].Text.Should().Be(ReplyMarkup.NextPageButtonText);
    }

    [Fact]
    public async Task MiddlePage_HasNextPage_ReturnsCorrectKeyboard()
    {
        // Arrange

        var contextMock = new DbContextMock<AppDbContext>(new DbContextOptions<AppDbContext>());
        var categories = new List<Category>();

        _fixture.AddManyTo(categories, 5);

        contextMock.CreateDbSetMock(m => m.Categories, categories);

        var handler = new GetCategoriesMarkupHandler(contextMock.Object, _mediator);

        var getCategoriesMarkup = new GetCategoriesMarkup(_itemId, 1);

        // Act

        var inlineKeyboardMarkup = await handler.Handle(getCategoriesMarkup, CancellationToken.None);

        // Assert

        const int categoriesRowsCount = 2;
        inlineKeyboardMarkup.InlineKeyboard.Count().Should().Be(categoriesRowsCount + 1);

        foreach (var buttonsRow in inlineKeyboardMarkup.InlineKeyboard.Take(categoriesRowsCount))
        {
            buttonsRow.Count().Should().Be(ButtonsPerRow);
        }

        var lastRowButtons = inlineKeyboardMarkup.InlineKeyboard.Last().ToImmutableArray();

        lastRowButtons.Length.Should().Be(3);

        lastRowButtons[0].Text.Should().Be(ReplyMarkup.PreviousPageButtonText);
        lastRowButtons[1].Text.Should().Be(ReplyMarkup.RefreshButtonText);
        lastRowButtons[2].Text.Should().Be(ReplyMarkup.NextPageButtonText);
    }

    [Fact]
    public async Task LastPage_CountLessPageSize_ReturnsCorrectKeyboard()
    {
        // Arrange

        var contextMock = new DbContextMock<AppDbContext>(new DbContextOptions<AppDbContext>());
        var categories = new List<Category>();

        _fixture.AddManyTo(categories, 3);

        contextMock.CreateDbSetMock(m => m.Categories, categories);

        var handler = new GetCategoriesMarkupHandler(contextMock.Object, _mediator);

        var getCategoriesMarkup = new GetCategoriesMarkup(_itemId, 1);

        // Act

        var inlineKeyboardMarkup = await handler.Handle(getCategoriesMarkup, CancellationToken.None);

        // Assert

        const int categoriesRowsCount = 1;
        inlineKeyboardMarkup.InlineKeyboard.Count().Should().Be(categoriesRowsCount + 1);

        foreach (var buttonsRow in inlineKeyboardMarkup.InlineKeyboard.Take(categoriesRowsCount))
        {
            buttonsRow.Count().Should().Be(ButtonsPerRow);
        }

        var lastRowButtons = inlineKeyboardMarkup.InlineKeyboard.Last().ToImmutableArray();

        lastRowButtons.Length.Should().Be(2);

        lastRowButtons[0].Text.Should().Be(ReplyMarkup.PreviousPageButtonText);
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

        var handler = new GetCategoriesMarkupHandler(contextMock.Object, _mediator);

        var getCategoriesMarkup = new GetCategoriesMarkup(_itemId, 1);

        // Act

        var inlineKeyboardMarkup = await handler.Handle(getCategoriesMarkup, CancellationToken.None);

        // Assert

        var categoriesRowsCount = 2;
        inlineKeyboardMarkup.InlineKeyboard.Count().Should().Be(categoriesRowsCount + 1);

        foreach (var buttonsRow in inlineKeyboardMarkup.InlineKeyboard.Take(categoriesRowsCount))
        {
            buttonsRow.Count().Should().Be(ButtonsPerRow);
        }

        var lastRowButtons = inlineKeyboardMarkup.InlineKeyboard.Last().ToImmutableArray();

        lastRowButtons.Length.Should().Be(2);

        lastRowButtons[0].Text.Should().Be(ReplyMarkup.PreviousPageButtonText);
        lastRowButtons[1].Text.Should().Be(ReplyMarkup.RefreshButtonText);
    }
}