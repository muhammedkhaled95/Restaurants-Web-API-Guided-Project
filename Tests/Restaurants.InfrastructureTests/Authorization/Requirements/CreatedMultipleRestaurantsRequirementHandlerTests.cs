using Castle.Core.Logging;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Restaurants.Application.Users;
using Restaurants.Domain.Constants;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Repositories;
using Xunit;

namespace Restaurants.Infrastructure.Authorization.Requirements.Tests;

public class CreatedMultipleRestaurantsRequirementHandlerTests
{
    [Fact()]
    public async Task HandleRequirementAsync_UserHasCreatedMultipleRestaurants_ShouldSucceed()
    {
        // Arrange
        var userContextMock = new Mock<IUserContext>();
        var currentUser = new CurrentUser("1", "test@test.com", [], null, null);
        userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);

        //Creating multiple restaurants where 2 of them has their owner Id to test the success scenario.
        var restaurants = new List<Restaurant>()
            {
                new()
                {
                    OwnerId = currentUser.Id,
                },
                new()
                {
                    OwnerId = currentUser.Id,
                },
                new()
                {
                    OwnerId = "2",
                },
            };

        var restaurantsRepositoryMock = new Mock<IRestaurantsRepository>();
        restaurantsRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(restaurants);

        var requirement = new CreatedMultipleRestaurantsRequirement(2);
        var handler = new CreatedMultipleRestaurantsRequirementHandler(restaurantsRepositoryMock.Object, userContextMock.Object, null);

        var authHandlerContext = new AuthorizationHandlerContext([requirement], null, null);
        // Act
        await handler.HandleAsync(authHandlerContext);

        // Assert
        authHandlerContext.HasSucceeded.Should().BeTrue();
    }

    [Fact()]
    public async Task HandleRequirementAsync_UserHasNotCreatedMultipleRestaurants_ShouldFail()
    {
        // Arrange
        var userContextMock = new Mock<IUserContext>();
        var currentUser = new CurrentUser("1", "test@test.com", [], null, null);
        userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);

        //Creating multiple restaurants where 2 of them has their owner Id to test the success scenario.
        var restaurants = new List<Restaurant>()
            {
                new()
                {
                    OwnerId = currentUser.Id,
                },
                new()
                {
                    OwnerId = "2",
                },
            };

        var restaurantsRepositoryMock = new Mock<IRestaurantsRepository>();
        restaurantsRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(restaurants);

        var requirement = new CreatedMultipleRestaurantsRequirement(2);
        var handler = new CreatedMultipleRestaurantsRequirementHandler(restaurantsRepositoryMock.Object, userContextMock.Object, null);

        var authHandlerContext = new AuthorizationHandlerContext([requirement], null, null);
        // Act
        await handler.HandleAsync(authHandlerContext);

        // Assert
        authHandlerContext.HasSucceeded.Should().BeFalse();
        authHandlerContext.HasFailed.Should().BeTrue();
    }
}