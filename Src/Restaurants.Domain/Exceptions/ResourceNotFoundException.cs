namespace Restaurants.Domain.Exceptions;
/*
 * In Clean Architecture, custom exceptions that represent domain-specific business logic
 * should be added to the Domain Layer. This ensures that business rules and invariants 
 * are encapsulated where they logically belong, making the code more maintainable and clear.
 * 
 * Here’s a brief breakdown of where to place exceptions:
 * 
 * 1. Domain Layer:
 *    - Contains exceptions that enforce business rules.
 *    - Example: An exception for when a product is out of stock.
 * 
 * 2. Application Layer:
 *    - Contains exceptions that are related to application-specific concerns,
 *      like validation failures or issues with external services.
 * 
 * 3. Infrastructure Layer:
 *    - Handles low-level technical exceptions (e.g., database, network issues),
 *      but these are typically caught and transformed rather than defined as custom exceptions.
 * 
 * Best Practices:
 * - Keep domain exceptions limited to business logic errors.
 * - Use middleware to handle exceptions globally rather than scattering try/catch blocks.
 * - Translate technical exceptions into domain exceptions at layer boundaries.
 */
public class ResourceNotFoundException : Exception
{
    public ResourceNotFoundException(string resourceType, string resourceIdentifier) : base($"{resourceType} with id = {resourceIdentifier} doesn't exist")
    {
        
    }
}
