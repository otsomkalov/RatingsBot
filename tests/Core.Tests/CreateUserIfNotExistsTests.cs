using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Core.Handlers.User;
using Core.Models;
using Core.Requests.User;
using Core.Services.Interfaces;
using Data;
using EntityFrameworkCoreMock.NSubstitute;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace Core.Tests;

public class CreateUserIfNotExistsTests
{
    private readonly string _userFirstName;
    private readonly int _userId;
    private readonly IUserIdProvider _userIdProvider;

    public CreateUserIfNotExistsTests()
    {
        _userIdProvider = Substitute.For<IUserIdProvider>();

        var fixture = new Fixture();

        _userId = fixture.Create<int>();
        _userFirstName = fixture.Create<string>();
    }

    [Fact]
    public async Task CachedUser_Works()
    {
        // Arrange

        _userIdProvider.UserExists(_userId).Returns(true);

        var contextMock = new DbContextMock<AppDbContext>(new DbContextOptions<AppDbContext>());

        var usersSetMock = contextMock.CreateDbSetMock(m => m.Users);

        var handler = new CreateIfNotExistsHandler(_userIdProvider, contextMock.Object);

        var request = new CreateUserIfNotExists(_userId, _userFirstName);

        // Act

        await handler.Handle(request, CancellationToken.None);

        // Assert

        _userIdProvider.Received().UserExists(_userId);
        var usersExists = await usersSetMock.Object.AnyAsync();
        usersExists.Should().BeFalse();
    }

    [Fact]
    public async Task ExistingNotCachedUser_Works()
    {
        // Arrange

        _userIdProvider.UserExists(_userId).Returns(false);

        var contextMock = new DbContextMock<AppDbContext>(new DbContextOptions<AppDbContext>());

        contextMock.CreateDbSetMock(m => m.Users, new[]
        {
            new User
            {
                Id = _userId
            }
        });

        var handler = new CreateIfNotExistsHandler(_userIdProvider, contextMock.Object);

        var request = new CreateUserIfNotExists(_userId, _userFirstName);

        // Act

        await handler.Handle(request, CancellationToken.None);

        // Assert

        _userIdProvider.Received().UserExists(_userId);
        _userIdProvider.Received().AddUserId(_userId);
    }

    [Fact]
    public async Task NotExistingNotCachedUser_Works()
    {
        // Arrange

        _userIdProvider.UserExists(_userId).Returns(false);

        var contextMock = new DbContextMock<AppDbContext>(new DbContextOptions<AppDbContext>());

        var usersSetMock = contextMock.CreateDbSetMock(m => m.Users);

        var handler = new CreateIfNotExistsHandler(_userIdProvider, contextMock.Object);

        var request = new CreateUserIfNotExists(_userId, _userFirstName);

        // Act

        await handler.Handle(request, CancellationToken.None);

        // Assert

        _userIdProvider.Received().UserExists(_userId);
        _userIdProvider.Received().AddUserId(_userId);
        await usersSetMock.Object.Received().AddAsync(Arg.Is<User>(u => u.Id == _userId && u.FirstName == _userFirstName));

        var count = await usersSetMock.Object.CountAsync();
        count.Should().Be(1);
    }
}