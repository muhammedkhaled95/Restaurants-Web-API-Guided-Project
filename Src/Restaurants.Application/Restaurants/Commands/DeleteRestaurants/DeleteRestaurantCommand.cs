using MediatR;

namespace Restaurants.Application.Restaurants.Commands.DeleteRestaurants;

public class DeleteRestaurantCommand(int id) : IRequest
{
    public int Id { get; } = id;
}
