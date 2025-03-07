using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Restaurants.API.Controllers.Tests;

public class RestaurantsControllerTests: IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _applicationFactory;
    public RestaurantsControllerTests(WebApplicationFactory<Program> applicationFactory)
    {
        _applicationFactory = applicationFactory;
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