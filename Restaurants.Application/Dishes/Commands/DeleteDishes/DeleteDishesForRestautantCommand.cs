using MediatR;

namespace Restaurants.Application.Dishes.Commands.DeleteDishes;

public class DeleteDishesForRestautantCommand(int restaurantId) : IRequest
{
    public int RestaurantId { get; set; } = restaurantId;
}
