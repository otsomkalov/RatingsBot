using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Handlers.Item;
using Core.Models;
using Core.Requests.Item;
using Data;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Core.Tests;

public class SetItemRatingTests
{
    [Fact]
    public async Task UpdateExistingRating_Works()
    {
        // Arrange

        var request = new SetItemRating(1, 1, 1);

        var contextMock = new DbContextMock<AppDbContext>(new DbContextOptions<AppDbContext>());

        var ratingsSetMock = contextMock.CreateDbSetMock(ctx => ctx.Ratings, new[]
        {
            new Rating
            {
                ItemId = 1,
                UserId = 1
            }
        });

        var handler = new SetItemRatingHandler(contextMock.Object);

        // Act

        await handler.Handle(request, CancellationToken.None);

        // Assert

        ratingsSetMock.Verify(set => set.Update(It.IsAny<Rating>()));
        var count = await contextMock.Object.Ratings.CountAsync();
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task CreateNewRating_Works()
    {
        // Arrange

        var request = new SetItemRating(1, 1, 1);

        var contextMock = new DbContextMock<AppDbContext>(new DbContextOptions<AppDbContext>());

        var ratingsSetMock = contextMock.CreateDbSetMock(ctx => ctx.Ratings, Array.Empty<Rating>());

        var handler = new SetItemRatingHandler(contextMock.Object);

        // Act

        await handler.Handle(request, CancellationToken.None);

        // Assert

        ratingsSetMock.Verify(set => set.AddAsync(It.IsAny<Rating>(), It.IsAny<CancellationToken>()));
        var count = await contextMock.Object.Ratings.CountAsync();
        Assert.Equal(1, count);
    }
}