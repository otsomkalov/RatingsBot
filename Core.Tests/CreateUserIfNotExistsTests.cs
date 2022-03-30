using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Commands.User;
using Core.Data;
using Core.Handlers.User;
using Core.Models;
using Core.Services.Interfaces;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Core.Tests;

public class CreateUserIfNotExistsTests
{
    private const long UserId = 1;
    private const string UserFirstName = nameof(UserFirstName);

    [Fact]
    public async Task CachedUser_Works()
    {
        // Arrange

        var userIdProviderMock = new Mock<IUserIdProvider>();

        userIdProviderMock.Setup(m => m.UserExists(It.IsAny<long>()))
            .Returns(true);

        var contextMock = new DbContextMock<AppDbContext>(new DbContextOptions<AppDbContext>());

        var usersSetMock = contextMock.CreateDbSetMock(m => m.Users, Array.Empty<User>());

        var handler = new CreateIfNotExistsHandler(userIdProviderMock.Object, contextMock.Object);

        var request = new CreateUserIfNotExists(UserId, UserFirstName);

        // Act

        await handler.Handle(request, CancellationToken.None);

        // Assert

        userIdProviderMock.Verify(m => m.UserExists(It.IsAny<long>()), Times.Once);

        var usersExists = await usersSetMock.Object.AnyAsync();
        Assert.False(usersExists);
    }

    [Fact]
    public async Task ExistingNotCachedUser_Works()
    {
        // Arrange

        var userIdProviderMock = new Mock<IUserIdProvider>();

        userIdProviderMock.Setup(m => m.UserExists(It.IsAny<long>()))
            .Returns(false);

        var contextMock = new DbContextMock<AppDbContext>(new DbContextOptions<AppDbContext>());

        contextMock.CreateDbSetMock(m => m.Users, new[]
        {
            new User
            {
                Id = UserId
            }
        });

        var handler = new CreateIfNotExistsHandler(userIdProviderMock.Object, contextMock.Object);

        var request = new CreateUserIfNotExists(UserId, UserFirstName);

        // Act

        await handler.Handle(request, CancellationToken.None);

        // Assert

        userIdProviderMock.Verify(m => m.UserExists(It.IsAny<long>()), Times.Once);
        userIdProviderMock.Verify(m => m.AddUserId(UserId), Times.Once);
    }

    [Fact]
    public async Task NotExistingNotCachedUser_Works()
    {
        // Arrange

        var userIdProviderMock = new Mock<IUserIdProvider>();

        userIdProviderMock.Setup(m => m.UserExists(It.IsAny<long>()))
            .Returns(false);

        var contextMock = new DbContextMock<AppDbContext>(new DbContextOptions<AppDbContext>());

        var usersSetMock = contextMock.CreateDbSetMock(m => m.Users, Array.Empty<User>());

        var handler = new CreateIfNotExistsHandler(userIdProviderMock.Object, contextMock.Object);

        var request = new CreateUserIfNotExists(UserId, UserFirstName);

        // Act

        await handler.Handle(request, CancellationToken.None);

        // Assert

        userIdProviderMock.Verify(m => m.UserExists(It.IsAny<long>()));
        userIdProviderMock.Verify(m => m.AddUserId(UserId));
        usersSetMock.Verify(m => m.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()));

        var userCreated = await usersSetMock.Object.AnyAsync(u => u.Id == UserId && u.FirstName == UserFirstName);
        Assert.True(userCreated);
    }
}