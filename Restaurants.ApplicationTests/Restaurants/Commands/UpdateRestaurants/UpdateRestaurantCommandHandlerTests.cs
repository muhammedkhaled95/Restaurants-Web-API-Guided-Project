using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Restaurants.Application.Restaurants.Commands.CreateRestaurants;
using Restaurants.Application.Users;
using Restaurants.Domain.Constants;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;
using Restaurants.Domain.Interfaces;
using Restaurants.Domain.Repositories;
using Xunit;

namespace Restaurants.Application.Restaurants.Commands.UpdateRestaurants.Tests;

public class UpdateRestaurantCommandHandlerTests
{
    private readonly Mock<ILogger<UpdateRestaurantCommandHandler>> _loggerMock;
    private readonly Mock<IRestaurantsRepository> _restaurantsRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRestaurantAuthorizationService> _restaurantAuthorizationServiceMock;
    private readonly UpdateRestaurantCommandHandler _handler;

    public UpdateRestaurantCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<UpdateRestaurantCommandHandler>>();
        _restaurantsRepositoryMock = new Mock<IRestaurantsRepository>();
        _mapperMock = new Mock<IMapper>();
        _restaurantAuthorizationServiceMock = new Mock<IRestaurantAuthorizationService>();
        _handler = new UpdateRestaurantCommandHandler(_loggerMock.Object, _mapperMock.Object,
                                                      _restaurantsRepositoryMock.Object,
                                                      _restaurantAuthorizationServiceMock.Object);
    }

    [Fact()]
    public async Task UpdateRestaurantTest_ValidRequest_ShouldUpdateRestaurantCorrectlyAsync()
    {
        // arrange
        var restaurantId = 1;
        var command = new UpdateRestaurantCommand()
        {
            Id = restaurantId,
            Name = "New Test",
            Description = "New Description",
            HasDelivery = true,
        };

        var restaurant = new Restaurant()
        {
            Id = restaurantId,
            Name = "Test",
            Description = "Test",
        };

        _restaurantsRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId))
            .ReturnsAsync(restaurant);

        _restaurantAuthorizationServiceMock.Setup(m => m.Authorize(restaurant, ResourceOperation.Update))
            .Returns(true);


        // act
        await _handler.Handle(command, CancellationToken.None);

        // Assert

        // Verify that the repository's SaveChanges() method was called exactly once.
        // This ensures that the handler attempted to persist the changes to the database.
        _restaurantsRepositoryMock.Verify(r => r.SaveChanges(), Times.Once);

        // Verify that the mapper's Map method was called exactly once with the command and the existing restaurant.
        // This ensures that the handler used the mapper to copy data from the command into the existing restaurant entity.
        _mapperMock.Verify(m => m.Map(command, restaurant), Times.Once);

    }

    [Fact()]
    public void UpdateRestaurantTest_NonExistingRestaurant_ShouldThrowResourceNotFoundException()
    {
        // Arrange: Set up the mock repository to simulate "restaurant not found"

        // This tells the mock `_restaurantsRepositoryMock` to return `null`
        // whenever `GetByIdAsync()` is called with *any integer argument*.
        // This simulates the case where the restaurant doesn't exist in the database.
        _restaurantsRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                                   .ReturnsAsync((Restaurant?)null);

        // Create an instance of the command that will be passed to the handler.
        // This command typically contains the data required to update a restaurant.
        var updateRestaurantCommand = new UpdateRestaurantCommand();

        // Act: Prepare to call the handler's Handle() method, but don't execute it immediately.

        // Here, we define `act` as a delegate (Func<Task>) that represents
        // calling `_handler.Handle()` with the command and a `CancellationToken`.
        // This deferred approach allows us to use FluentAssertions to check for exceptions.
        Func<Task> act = async () => await _handler.Handle(updateRestaurantCommand, CancellationToken.None);

        // Assert: Verify that executing `act` throws the expected exception.

        // This line asserts that calling the handler should result in a `ResourceNotFoundException`.
        // FluentAssertions captures the exception thrown from `Handle()` and compares it with the expected type.
        act.Should().ThrowAsync<ResourceNotFoundException>()
                    .WithMessage("Restaurant doesn't exist");

    }

    [Fact()]
    public void UpdateRestaurantTest_ForbiddenUpdateOfExistingRestaurant_ShouldThrowForbidException()
    {
        // / Arrange
        var restaurantId = 3;
        var updateRestaurantCommand = new UpdateRestaurantCommand
        {
            Id = restaurantId
        };

        var existingRestaurant = new Restaurant
        {
            Id = restaurantId
        };

        _restaurantsRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId))
                                  .ReturnsAsync(existingRestaurant);

        _restaurantAuthorizationServiceMock.Setup(a => a.Authorize(existingRestaurant, ResourceOperation.Update))
                                           .Returns(false);

        // Act
        Func<Task> act = async () => await _handler.Handle(updateRestaurantCommand, CancellationToken.None);

        // Assert
        act.Should().ThrowAsync<ForbidException>()
                    .WithMessage("User not allowed to update restaurant");

    }
}