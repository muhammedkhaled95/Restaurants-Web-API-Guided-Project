using System.ComponentModel.DataAnnotations;

namespace Restaurants.Application.Restaurants.DTOs;

public class CreateRestaurantDto
{
    //[StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = default!;

    public string Description { get; set; } = default!;

    //[Required(ErrorMessage = "Insert a valid category")]
    public string Category { get; set; } = default!;

    public bool HasDelivery { get; set; }

    //[EmailAddress(ErrorMessage = "Provide a valid email address")]
    public string ContactEmail { get; set; } = default!;
    
    public string ContactNumber { get; set; } = default!;
    
    public string? City { get; set; }
    
    public string? Street { get; set; }

    //[RegularExpression(@"^\d{2}-\d{3}$", ErrorMessage = "Provie a valid postal code (XX-XXX)")]
    public string? PostalCode { get; set; }

}


/*
 * 1. Attribute-based Validation (Data Annotations) which is the commented code in this file.
  * Uses attributes ([Required], [MaxLength], etc.) directly on model properties.
  * Validation rules are tightly coupled to the data model (e.g., DTOs or entities).
  * Simpler to implement for straightforward use cases but harder to extend or customize.
 */

/*
 * 2. Fluent Validation
  * Implements validation logic in separate validator classes, decoupling the validation rules from the model.
  * Provides a fluent, expressive API for defining rules.
  * More flexible, extensible, and testable compared to attribute-based validation.
  * Supports complex scenarios like conditional rules, custom validation logic, and localization.
 */