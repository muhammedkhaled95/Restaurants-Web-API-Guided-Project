using MediatR;

namespace Restaurants.Application.Restaurants.Commands.CreateRestaurants
{
    public class CreateRestaurantCommand : IRequest<int>
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Category { get; set; } = default!;
        public bool HasDelivery { get; set; }
        public string ContactEmail { get; set; } = default!;
        public string ContactNumber { get; set; } = default!;
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? PostalCode { get; set; }
    }
}
