using Microsoft.AspNetCore.Http;
using Restaurants.Domain.Constants;
using System.Security.Claims;

namespace Restaurants.Application.Users;

public interface IUserContext
{
    CurrentUser? GetCurrentUser();
}

/*
 * HttpContext in ASP.NET Core is like the "backstage pass" to everything about an HTTP request and response.
 * 🎟️ It provides access to all the info related to the current HTTP request, including details like:
    - Request data (headers, query strings, body, cookies)
    - Response data (status code, headers, cookies)
    - User information (via HttpContext.User)
    - Session and connection information
    - Features like authentication, authorization, and more
 */
public class UserContext : IUserContext
{
    /*
     * IHttpContextAccessor in ASP.NET Core is like a portal 🌀 that gives you access to the current HttpContext
       outside of controllers and middleware, where it’s not directly available (like in services or background tasks).
     * Without it, you'd be stuck thinking, "How do I get the current user's info or request details here?" 😫
     * With it, you’re like, “Oh hey HttpContext, didn’t expect to see you here!” 😎
     * Use it to get request info, user claims, headers, cookies, etc.
     * ⚠️ HttpContext is request-scoped; DO NOT store it in singleton services.
     * ✅ Access it only when needed to avoid thread-safety issues.
     */
    private readonly IHttpContextAccessor _contextAccessor;
    public UserContext(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }
    public CurrentUser? GetCurrentUser()
    {
        var user = _contextAccessor?.HttpContext?.User;

        if (user == null)
        {
            throw new InvalidOperationException("User Context doesn't exist");
        }

        if (user.Identity == null || !user.Identity.IsAuthenticated)
        {
            return null;
        }

        var userId = user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var email = user.FindFirst(c => c.Type == ClaimTypes.Email)?.Value;
        var roles = user.Claims.Where(c => c.Type == ClaimTypes.Role)?.Select(c => c.Value);
        var nationality = user.FindFirst(c => c.Type == "Nationality")?.Value;
        var dobstring = user.FindFirst(c => c.Type == "DateOfBirth")?.Value;
        var dob = dobstring == null? (DateOnly?)null : DateOnly.ParseExact(dobstring, "yyyy-MM-dd");
        return new CurrentUser(userId, email, roles, nationality, dob);
    }
}
