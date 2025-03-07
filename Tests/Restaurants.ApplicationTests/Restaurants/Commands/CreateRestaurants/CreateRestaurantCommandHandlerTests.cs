using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Restaurants.Application.Users;
using Restaurants.Domain.Constants;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Repositories;
using Xunit;

namespace Restaurants.Application.Restaurants.Commands.CreateRestaurants.Tests;

/// <summary>
/// Unit test class for CreateRestaurantCommandHandler.
/// This class contains tests to verify the behavior of the CreateRestaurantCommandHandler when handling
/// the creation of a new restaurant.
/// </summary>
public class CreateRestaurantCommandHandlerTests
{
    /// <summary>
    /// Tests that the CreateRestaurantCommandHandler correctly handles a valid CreateRestaurantCommand
    /// and returns the created restaurant's ID.
    /// </summary>
    [Fact]
    public async void Handle_ForValidCommand_ReturnsCreatedRestaurantId()
    {
        // Arrange
        // 1. Mock a logger to satisfy the command handler's dependency on ILogger.
        var loggerMock = new Mock<ILogger<CreateRestaurantCommandHandler>>();

        // 2. Mock the IMapper dependency. The command handler uses AutoMapper to map the command into a Restaurant entity.
        var mapperMock = new Mock<IMapper>();

        // Create an instance of the command being tested (CreateRestaurantCommand).
        var createRestaurantCommand = new CreateRestaurantCommand();

        // Create an empty restaurant object to simulate the result of the mapping process.
        var restaurant = new Restaurant();

        // Set up the mapper mock to return the 'restaurant' object whenever the mapping method is called.
        mapperMock.Setup(m => m.Map<Restaurant>(createRestaurantCommand)).Returns(restaurant);

        // 3. Mock the restaurant repository, which handles the actual creation of the restaurant in the database.
        var restaurantRepositoryMock = new Mock<IRestaurantsRepository>();

        // Set up the repository mock to return a hardcoded restaurant ID (1) when the Create method is called.
        restaurantRepositoryMock.Setup(repo => repo.Create(It.IsAny<Restaurant>()))
                                .ReturnsAsync(1);

        // 4. Mock the IUserContext service to provide information about the currently logged-in user.
        var userContextMock = new Mock<IUserContext>();

        // Simulate a logged-in user with ID "1", email "test@test.com", and roles of Admin and User.
        var currentUser = new CurrentUser("1", "test@test.com", [UserRoles.Admin, UserRoles.User], null, null);

        // Set up the user context mock to always return this fake current user.
        userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);

        // 5. Instantiate the command handler with all the mocked dependencies.
        var createCommandHandler = new CreateRestaurantCommandHandler(
            loggerMock.Object,
            mapperMock.Object,
            restaurantRepositoryMock.Object,
            userContextMock.Object
        );

        // Act
        // Call the handler's Handle method, simulating the process of handling the command.
        var result = await createCommandHandler.Handle(createRestaurantCommand, CancellationToken.None);

        // Assert
        // 1. Verify the returned restaurant ID is 1, which is what the repository mock was set to return.
        result.Should().Be(1);

        // 2. Ensure the restaurant's OwnerId was set to the current user's ID ("1").
        restaurant.OwnerId.Should().Be("1");

        // 3. Verify the Create method was called exactly once on the repository with the mapped restaurant object.
        restaurantRepositoryMock.Verify(repo => repo.Create(restaurant), Times.Once);
    }
}
