using MediatR;

namespace Restaurants.Application.Users.Commands.AssignUserRole;

public class AssignUserRoleCommand : IRequest
{
    public string userEmail { get; set; } = default!;
    public string RoleName { get; set; } = default!;
}
