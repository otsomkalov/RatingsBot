using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Dsl;
using Core.Handlers.Item;
using Core.Models;
using Core.Requests.Item;
using Data;
using EntityFrameworkCoreMock.NSubstitute;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace Core.Tests;

public class SetItemRatingTests
{
    private readonly IFixture _fixture;
    private readonly int _itemId;

    private readonly IPostprocessComposer<Rating> _ratingComposer;
    private readonly int _ratingValue;
    private readonly long _userId;

    public SetItemRatingTests()
    {
        _fixture = new Fixture();

        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _itemId = _fixture.Create<int>();
        _userId = _fixture.Create<long>();
        _ratingValue = _fixture.Create<int>();

        _ratingComposer = _fixture.Build<Rating>()
            .With(r => r.UserId, _userId)
            .With(r => r.ItemId, _itemId);
    }

    [Fact]
    public async Task UpdateExistingRating_SameRatingValue_DoesntAffectDb()
    {
        // Arrange

        var request = new SetItemRating(_itemId, _userId, _ratingValue);

        var contextMock = new DbContextMock<AppDbContext>(new DbContextOptions<AppDbContext>());

        var rating = _ratingComposer
            .With(r => r.Value, _ratingValue)
            .Create();

        var ratingsSetMock = contextMock.CreateDbSetMock(ctx => ctx.Ratings, new[]
        {
            rating
        });

        var handler = new SetItemRatingHandler(contextMock.Object);

        // Act

        await handler.Handle(request, CancellationToken.None);

        // Assert

        ratingsSetMock.Object.DidNotReceive().Update(Arg.Any<Rating>());
        await ratingsSetMock.Object.DidNotReceive().AddAsync(Arg.Any<Rating>());
        await contextMock.Object.DidNotReceive().SaveChangesAsync();

        var count = await contextMock.Object.Ratings.CountAsync();
        count.Should().Be(1);
    }

    [Fact]
    public async Task UpdateExistingRating_Works()
    {
        // Arrange

        var request = new SetItemRating(_itemId, _userId, _ratingValue);

        var contextMock = new DbContextMock<AppDbContext>(new DbContextOptions<AppDbContext>());

        var rating = _ratingComposer
            .With(r => r.Value, _fixture.Create<int>())
            .Create();

        var ratingsSetMock = contextMock.CreateDbSetMock(ctx => ctx.Ratings, new[]
        {
            rating
        });

        var handler = new SetItemRatingHandler(contextMock.Object);

        // Act

        await handler.Handle(request, CancellationToken.None);

        // Assert

        ratingsSetMock.Object.Received().Update(Arg.Any<Rating>());
        var count = await contextMock.Object.Ratings.CountAsync();
        count.Should().Be(1);
    }

    [Fact]
    public async Task CreateNewRating_Works()
    {
        // Arrange

        var request = new SetItemRating(_itemId, _userId, _ratingValue);

        var contextMock = new DbContextMock<AppDbContext>(new DbContextOptions<AppDbContext>());

        var ratingsSetMock = contextMock.CreateDbSetMock(ctx => ctx.Ratings);

        var handler = new SetItemRatingHandler(contextMock.Object);

        // Act

        await handler.Handle(request, CancellationToken.None);

        // Assert

        await ratingsSetMock.Object.Received()
            .AddAsync(Arg.Is<Rating>(r => r.ItemId == _itemId && r.UserId == _userId && r.Value == _ratingValue));

        await contextMock.Object.Received().SaveChangesAsync();
        var count = await contextMock.Object.Ratings.CountAsync();
        count.Should().Be(1);
    }
}