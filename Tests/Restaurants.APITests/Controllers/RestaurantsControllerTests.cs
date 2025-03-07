using FluentAssertions;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Restaurants.API.Tests;
using Restaurants.Application.Restaurants.DTOs;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Repositories;
using Restaurants.Infrastructure.Seeders;
using System.Net.Http.Json;
using System.Net;
using Xunit;

namespace Restaurants.API.Controllers.Tests;

public class RestaurantsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _applicationFactory;
    private readonly Mock<IRestaurantsRepository> _restaurantsRepositoryMock = new();
    private readonly Mock<IRestaurantSeeder> _restaurantsSeederMock = new();
    public RestaurantsControllerTests(WebApplicationFactory<Program> factory)
    {
        _applicationFactory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
                services.Replace(ServiceDescriptor.Scoped(typeof(IRestaurantsRepository),
                                            _ => _restaurantsRepositoryMock.Object));


                services.Replace(ServiceDescriptor.Scoped(typeof(IRestaurantSeeder),
                                            _ => _restaurantsSeederMock.Object));
            });
        });
    }

    [Fact()]
    public async void GetByIdTest_ForNonExistingId_ShouldReturnStatusCode404NotFound()
    {
        // Arrange
        var id = 1123;

        _restaurantsRepositoryMock.Setup(m => m.GetByIdAsync(id)).ReturnsAsync((Restaurant?)null);
        
        var client = _applicationFactory.CreateClient();

        // Act
        var response = await client.GetAsync($"api/restaurants/{id}");
        
        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_ForExistingId_ShouldReturn200Ok()
    {
        // arrange

        var id = 99;

        var restaurant = new Restaurant()
        {
            Id = id,
            Name = "Test",
            Description = "Test description"
        };

        _restaurantsRepositoryMock.Setup(m => m.GetByIdAsync(id)).ReturnsAsync(restaurant);

        var client = _applicationFactory.CreateClient();

        // act
        var response = await client.GetAsync($"/api/restaurants/{id}");
        var restaurantDto = await response.Content.ReadFromJsonAsync<RestaurantDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        restaurantDto.Should().NotBeNull();
        restaurantDto.Name.Should().Be("Test");
        restaurantDto.Description.Should().Be("Test description");
    }


    [Fact()]
    public async void GetAllTest_ForValidRequest_ShouldReturnStatusCode200OK()
    {
        // Arrange
        var client = _applicationFactory.CreateClient();

        // Act
        var result = await client.GetAsync("api/restaurants?pageNumber=1&pageSize=10");

        // Assert
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact()]
    public async void GetAllTest_ForInvalidRequest_ShouldReturnStatusCode400BadRequest()
    {
        // Arrange
        var client = _applicationFactory.CreateClient();

        // Act
        var result = await client.GetAsync("api/restaurants");

        // Assert
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }
}