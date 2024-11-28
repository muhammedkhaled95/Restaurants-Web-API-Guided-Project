using System.ComponentModel.DataAnnotations;

namespace Restaurants.Application.Restaurants.DTOs;

public class CreateRestaurantDto
{
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = default!;

    public string Description { get; set; } = default!;

    [Required(ErrorMessage = "Insert a valid category")]
    public string Category { get; set; } = default!;

    public bool HasDelivery { get; set; }

    [EmailAddress(ErrorMessage = "Provide a valid email address")]
    public string ContactEmail { get; set; } = default!;
    
    public string ContactNumber { get; set; } = default!;
    
    public string? City { get; set; }
    
    public string? Street { get; set; }

    [RegularExpression(@"^\d{2}-\d{3}$", ErrorMessage = "Provie a valid postal code (XX-XXX)")]
    public string? PostalCode { get; set; }

}
