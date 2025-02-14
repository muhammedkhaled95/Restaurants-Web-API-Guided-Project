using MediatR;

namespace Restaurants.Application.Restaurants.Commands.DeleteRestaurants;

public class DeleteRestaurantCommand(int id) : IRequest<bool>
{
    public int Id { get; } = id;
}
