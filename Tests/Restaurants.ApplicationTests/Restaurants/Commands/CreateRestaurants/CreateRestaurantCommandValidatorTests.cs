using FluentValidation.TestHelper;
using Restaurants.Application.Restaurants.Commands.CreateRestaurants;
using Xunit;

namespace Restaurants.Application.Restaurants.Validators.Tests;

public class CreateRestaurantCommandValidatorTests
{
    // Naming convention explanation:
    // [MethodName]_GivenScenario_ExpectedOutcome
    [Fact()]
    public void ValidatorTest_ForValidCommand_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new CreateRestaurantCommand()
        {
            Name = "Test",
            Description = "Test Description",
            Category = "Italian",
            ContactEmail = "test@test.com",
            PostalCode = "12-345",
        };

        var validator = new CreateRestaurantCommandValidator();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact()]
    public void ValidatorTest_ForInvalidCommand_ShouldHaveValidationErrors()
    {
        // Arrange
        var command = new CreateRestaurantCommand()
        {
            Name = "T",
            Description = null,
            Category = "Colombian",
            ContactEmail = "test-test.com",
            PostalCode = "12-3245",
        };

        var validator = new CreateRestaurantCommandValidator();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Name);
        result.ShouldHaveValidationErrorFor(c => c.Description);
        result.ShouldHaveValidationErrorFor(c => c.Category);
        result.ShouldHaveValidationErrorFor(c => c.ContactEmail);
        result.ShouldHaveValidationErrorFor(c => c.PostalCode);
    }

    [Theory()]
    [InlineData("Mexican")]
    [InlineData("Italian")]
    [InlineData("Egyptian")]
    [InlineData("Indian")]
    [InlineData("Chinese")]
    [InlineData("French")]
    public void ValidatorTest_ForValidCategory_ShouldNotHaveValidationErrorsForCategoryProperty(string category)
    {
        // Arrange
        var command = new CreateRestaurantCommand() { Category = category };

        var validator = new CreateRestaurantCommandValidator();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.Category);
    }
}