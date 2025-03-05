using FluentAssertions;                  // FluentAssertions is a popular assertion library for making assertions more readable and expressive.
using Microsoft.AspNetCore.Http;         // Provides access to HttpContext and related types (used for accessing the current HTTP request/response).
using Moq;                               // Moq is a mocking framework that allows you to create fake/mock versions of dependencies for testing.
using Restaurants.Domain.Constants;      // Reference to your project's constants (where UserRoles.Admin, UserRoles.User are defined).
using System.Security.Claims;            // Provides classes like ClaimsPrincipal and Claim to represent authenticated users.
using Xunit;                             // Xunit is the unit testing framework you're using.

namespace Restaurants.Application.Users.Tests  // Namespace organizing the tests under your application's domain.
{
    /// <summary>
    /// Unit tests for the UserContext class, responsible for retrieving the current authenticated user.
    /// </summary>
    public class UserContextTests
    {
        /// <summary>
        /// Verifies that GetCurrentUser correctly extracts user information (ID, email, roles, nationality, date of birth)
        /// from the HttpContext's User object when the user is authenticated.
        /// </summary>
        [Fact]
        public void GetCurrentUserTest_WithAuthenticatedUser_ShouldReturnCurrentUser()
        {
            // Arrange - Set up test data, mocks, and the system under test (UserContext).

            // Define a sample date of birth to add to the user's claims.
            var dateOfBirth = new DateOnly(1990, 10, 30);

            // Create a mock IHttpContextAccessor.
            // IHttpContextAccessor is the interface for accessing the current HttpContext.
            // We're mocking it so we can control the HttpContext for testing purposes.
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            // Create a list of claims to simulate the authenticated user.
            // Claims represent the authenticated user's properties like ID, email, roles, etc.
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),                         // User ID claim.
                new Claim(ClaimTypes.Email, "mocktest@unittest.com"),              // User email claim.
                new Claim(ClaimTypes.Role, UserRoles.Admin),                       // User role claim (Admin).
                new Claim(ClaimTypes.Role, UserRoles.User),                        // User role claim (User).
                new Claim("Nationality", "German"),                               // Custom claim for nationality.
                new Claim("DateOfBirth", dateOfBirth.ToString("yyyy-MM-dd"))      // Custom claim for date of birth.
            };

            // Create a ClaimsPrincipal (the authenticated user) based on the claims.
            // ClaimsIdentity represents the identity part of the user (name, roles, etc.).
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));

            // Set up the mock IHttpContextAccessor to return a HttpContext
            // where the User is set to the ClaimsPrincipal we just created.
            httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext()
            {
                User = user
            });

            // Create an instance of UserContext, passing the mocked IHttpContextAccessor.
            // This ensures UserContext will read the user from our mock HttpContext.
            var userContext = new UserContext(httpContextAccessorMock.Object);

            // Act - Call the method under test.
            var currentUser = userContext.GetCurrentUser();

            // Assert - Verify the method behaved as expected.
            currentUser.Should().NotBeNull();                        // The returned user should not be null.
            currentUser.Id.Should().Be("1");                         // Check if the ID matches the claim.
            currentUser.Email.Should().Be("mocktest@unittest.com");  // Check if the email matches the claim.
            currentUser.Roles.Should().ContainInOrder(               // Check if the roles contain both Admin and User.
                UserRoles.Admin,
                UserRoles.User
            );
            currentUser.Nationality.Should().Be("German");           // Check if the nationality matches the claim.
            currentUser.DateOfBirth.Should().Be(dateOfBirth);        // Check if the date of birth matches the claim.
        }

        /// <summary>
        /// Verifies that GetCurrentUser throws an InvalidOperationException
        /// when there is no HttpContext (indicating the request is not in a valid user context).
        /// </summary>
        [Fact]
        public void GetCurrentUserTest_WithUserContextNotPresent_ThrowsInvalidationException()
        {
            // Arrange - Set up mocks and the system under test.

            // Create a mock IHttpContextAccessor.
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            // Set up the mock to return null HttpContext.
            // This simulates the case where the code is running outside of a valid HTTP request (for example, background services).
            httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext?)null);

            // Create an instance of UserContext with the mocked accessor.
            var userContext = new UserContext(httpContextAccessorMock.Object);

            // Act - Define the action that calls GetCurrentUser().
            // This is wrapped in an Action so we can assert that it throws.
            Action action = () => userContext.GetCurrentUser();

            // Assert - Verify that the expected exception is thrown.
            action.Should().Throw<InvalidOperationException>();  // We expect an InvalidOperationException.
        }
    }
}
