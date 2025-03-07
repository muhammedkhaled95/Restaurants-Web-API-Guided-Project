using MediatR;

namespace Restaurants.Application.Users.Commands.UpdateUsers;

public class UpdateUserDetailsCommand : IRequest
{
    public DateOnly? DateOfBirth { get; set; }
    public string? Nationality { get; set; }
}
