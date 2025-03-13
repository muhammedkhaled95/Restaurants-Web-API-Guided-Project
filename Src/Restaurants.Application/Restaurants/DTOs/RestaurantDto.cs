using Restaurants.Application.Dishes.DTOs;
using Restaurants.Domain.Entities;

namespace Restaurants.Application.Restaurants.DTOs
{
    public class RestaurantDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Category { get; set; } = default!;
        public bool HasDelivery { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? PostalCode { get; set; }
        public List<DishDto> dishes { get; set; } = [];

        public string? LogoSASUrl { get; set; } = default!;
        public static RestaurantDto? MapEntityToDto(Restaurant? restaurant)
        {
            if (restaurant == null) return null;
            return new RestaurantDto
            {
                Category = restaurant.Category,
                Description = restaurant.Description,
                Id = restaurant.Id,
                Name = restaurant.Name,
                City = restaurant.Address?.City,
                Street = restaurant.Address?.Street,
                PostalCode = restaurant.Address?.PostalCode,
                dishes = restaurant.dishes.Select(DishDto.MapEntityToDto).ToList(),
            };
        }
    }
}
