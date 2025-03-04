using FluentAssertions;
using Restaurants.Domain.Constants;
using Xunit;

namespace Restaurants.Application.Users.Tests;

/// <summary>
/// Unit tests for the <see cref="CurrentUser"/> class.
/// </summary>
public class CurrentUserTests
{
    // Naming convention explanation:
    // [MethodName]_GivenScenario_ExpectedOutcome
    // Example: IsInRole_WithMatchingRole_ShouldReturnTrue
    // This makes it clear what the method does, under which condition, and what you expect to happen.

    /// <summary>
    /// Tests that <see cref="CurrentUser.IsInRole"/> correctly returns <c>true</c>
    /// when the current user has a role that matches the provided role name.
    /// 
    /// This test uses the <see cref="TheoryAttribute"/> and <see cref="InlineDataAttribute"/>
    /// to test multiple roles (Admin and User) with the same logic.
    /// </summary>
    /// <param name="roleName">The role to check for (passed from <see cref="InlineDataAttribute"/>)</param>
    [Theory]
    [InlineData(UserRoles.Admin)]
    [InlineData(UserRoles.User)]
    public void IsInRole_WithMatchingRole_ShouldReturnTrue(string roleName)
    {
        // Arrange: Set up the CurrentUser with roles including Admin.
        var currentUser = new CurrentUser("1", "test@test.com", [UserRoles.Admin, UserRoles.User], null, null);

        // Act: Check if the user is in the Admin role.
        var isInRole = currentUser.IsInRole(roleName);

        // Assert: Verify that the result is true (the user is indeed in the Admin role).
        isInRole.Should().BeTrue();
    }

    [Fact]
    public void IsInRole_WithNoMatchingRole_ShouldReturnFalse()
    {
        // Arrange: Set up the CurrentUser with roles including Admin.
        var currentUser = new CurrentUser("1", "test@test.com", [UserRoles.Admin, UserRoles.User], null, null);

        // Act: Check if the user is in the owner role.
        var isInRole = currentUser.IsInRole(UserRoles.Owner);

        // Assert: Verify that the result is false (the user is not in the owner role).
        isInRole.Should().BeFalse();
    }

    [Fact]
    public void IsInRole_WithNoMatchingRoleCase_ShouldReturnFalse()
    {
        // Arrange: Set up the CurrentUser with roles including Admin.
        var currentUser = new CurrentUser("1", "test@test.com", [UserRoles.Admin, UserRoles.User], null, null);

        // Act: Check if the user is in the Admin role with a lower case.
        var isInRole = currentUser.IsInRole(UserRoles.Admin.ToLower());

        // Assert: Verify that the result is false (the user is indeed in the Admin role but with lower case).
        isInRole.Should().BeFalse();
    }
}
