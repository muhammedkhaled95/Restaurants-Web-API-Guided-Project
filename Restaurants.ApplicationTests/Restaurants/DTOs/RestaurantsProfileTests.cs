using AutoMapper;
using FluentAssertions;
using Restaurants.Application.Restaurants.Commands.CreateRestaurants;
using Restaurants.Application.Restaurants.Commands.UpdateRestaurants;
using Restaurants.Domain.Entities;
using Xunit;

namespace Restaurants.Application.Restaurants.DTOs.Tests;

public class RestaurantsProfileTests
{
    private readonly IMapper _mapper;

    public RestaurantsProfileTests()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<RestaurantsProfile>();
        });

        _mapper = configuration.CreateMapper();
    }

    [Fact()]
    public void CreateMap_ForRestaurantToRestaurantDto_MapsCorrectly()
    {
        // Arrange
        var restaurant = new Restaurant()
        {
            Id = 1,
            Name = "Test restaurant",
            Description = "Test Description",
            Category = "Test Category",
            HasDelivery = true,
            ContactEmail = "test@example.com",
            ContactNumber = "123456789",
            Address = new Address
            {
                City = "Test City",
                Street = "Test Street",
                PostalCode = "12-345"
            }
        };

        // Act
        var restaurantDto = _mapper.Map<RestaurantDto>(restaurant);

        // Assert
        restaurantDto.Should().NotBeNull();
        restaurantDto.Id.Should().Be(restaurant.Id);
        restaurantDto.Name.Should().Be(restaurant.Name);
        restaurantDto.Description.Should().Be(restaurant.Description);
        restaurantDto.Category.Should().Be(restaurant.Category);
        restaurantDto.HasDelivery.Should().Be(restaurant.HasDelivery);
        restaurantDto.City.Should().Be(restaurant.Address.City);
        restaurantDto.Street.Should().Be(restaurant.Address.Street);
        restaurantDto.PostalCode.Should().Be(restaurant.Address.PostalCode);
    }

    [Fact()]
    public void CreateMap_ForCreateRestaurantToRestaurant_MapsCorrectly()
    {
        // Arrange
      
        var createRestaurantCommand = new CreateRestaurantCommand()
        {
            Name = "Test restaurant",
            Description = "Test Description",
            Category = "Test Category",
            HasDelivery = true,
            ContactEmail = "test@example.com",
            ContactNumber = "123456789",
            City = "Test City",
            Street = "Test Street",
            PostalCode = "12-345"
        };

        // Act
        var restaurant = _mapper.Map<Restaurant>(createRestaurantCommand);

        // Assert
        restaurant.Should().NotBeNull();
        restaurant.Name.Should().Be(createRestaurantCommand.Name);
        restaurant.Description.Should().Be(createRestaurantCommand.Description);
        restaurant.Category.Should().Be(createRestaurantCommand.Category);
        restaurant.HasDelivery.Should().Be(createRestaurantCommand.HasDelivery);
        restaurant.Address.City.Should().Be(createRestaurantCommand.City);
        restaurant.Address.Street.Should().Be(createRestaurantCommand.Street);
        restaurant.Address.PostalCode.Should().Be(createRestaurantCommand.PostalCode);
    }

    [Fact()]
    public void CreateMap_ForUpdateRestaurantToRestaurant_MapsCorrectly()
    {
        // Arrange

        var updateRestaurantCommand = new UpdateRestaurantCommand()
        {
            Id = 1,
            Name = "Test restaurant",
            Description = "Test Description",
            HasDelivery = true
        };

        // Act
        var restaurant = _mapper.Map<Restaurant>(updateRestaurantCommand);

        // Assert
        restaurant.Should().NotBeNull();
        restaurant.Id.Should().Be(updateRestaurantCommand.Id);
        restaurant.Name.Should().Be(updateRestaurantCommand.Name);
        restaurant.Description.Should().Be(updateRestaurantCommand.Description);
        restaurant.HasDelivery.Should().Be(updateRestaurantCommand.HasDelivery);
    }
}