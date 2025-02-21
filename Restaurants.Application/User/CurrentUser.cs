namespace Restaurants.Application.User;

public record CurrentUser(string Id, string Email, IEnumerable<string> Roles) 
{
    public bool IsInRole(string RoleName)
    {
        return Roles.Contains(RoleName);
    }
}
