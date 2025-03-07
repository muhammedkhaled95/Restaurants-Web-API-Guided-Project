using MediatR;
using Restaurants.Application.Dishes.DTOs;

namespace Restaurants.Application.Dishes.Queries.GetDishesForRestaurant;

public class GetDishesForRestaurantQuery : IRequest<IEnumerable<DishDto>>
{
    public int RestaurantId;
    public GetDishesForRestaurantQuery(int restaurantId)
    {
        RestaurantId = restaurantId;
    }
}
