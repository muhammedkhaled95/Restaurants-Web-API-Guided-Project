using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;
using Xunit;

namespace Restaurants.API.Middlewares.Tests;

public class ErrorHandlingMiddlewareTests
{
    [Fact()]
    public async void InvokeAsyncTest_WhenNoExceptionThrown_ShouldCallNextDelegate()
    {
        // Arrange 
        var loggerMock = new Mock<ILogger<ErrorHandlingMiddleware>>();
        var middleware = new ErrorHandlingMiddleware(loggerMock.Object);
        var context = new DefaultHttpContext();
        var nextDelegateMock = new Mock<RequestDelegate>();

        // Act
        await middleware.InvokeAsync(context, nextDelegateMock.Object);

        // Assert
        nextDelegateMock.Verify(next => next.Invoke(context), Times.Once);
    }

    [Fact()]
    public async void InvokeAsyncTest_WhenResourceNotFoundExceptionThrown_ShouldSetStatusCodeTo404()
    {
        // Arrange 
        var loggerMock = new Mock<ILogger<ErrorHandlingMiddleware>>();
        var middleware = new ErrorHandlingMiddleware(loggerMock.Object);
        var context = new DefaultHttpContext();
        var resourceNotFoundException = new ResourceNotFoundException(nameof(Restaurant), "1");
        
        // Act
        await middleware.InvokeAsync(context, _ => throw resourceNotFoundException);

        // Assert
        context.Response.StatusCode.Should().Be(404);
    }

    [Fact()]
    public async void InvokeAsyncTest_WhenForbidExceptionThrown_ShouldSetStatusCodeTo403()
    {
        // Arrange 
        var loggerMock = new Mock<ILogger<ErrorHandlingMiddleware>>();
        var middleware = new ErrorHandlingMiddleware(loggerMock.Object);
        var context = new DefaultHttpContext();
        var forbidException = new ForbidException();

        // Act
        await middleware.InvokeAsync(context, _ => throw forbidException);

        // Assert
        context.Response.StatusCode.Should().Be(403);
    }

    [Fact()]
    public async void InvokeAsyncTest_WhenGenericExceptionThrown_ShouldSetStatusCodeTo500()
    {
        // Arrange 
        var loggerMock = new Mock<ILogger<ErrorHandlingMiddleware>>();
        var middleware = new ErrorHandlingMiddleware(loggerMock.Object);
        var context = new DefaultHttpContext();
        var genericException = new Exception();

        // Act
        await middleware.InvokeAsync(context, _ => throw genericException);

        // Assert
        context.Response.StatusCode.Should().Be(500);
    }
}