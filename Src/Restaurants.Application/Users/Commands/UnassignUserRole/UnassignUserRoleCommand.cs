using MediatR;

namespace Restaurants.Application.Users.Commands.UnassignUserRole;

public class UnassignUserRoleCommand : IRequest
{
    public string userEmail { get; set; } = default!;
    public string RoleName { get; set; } = default!;
}
