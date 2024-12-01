using FluentValidation;
using Restaurants.Application.Restaurants.Commands.CreateRestaurants;

namespace Restaurants.Application.Restaurants.Validators;

public class CreateRestaurantCommandValidator : AbstractValidator<CreateRestaurantCommand>
{
    private readonly List<String> Categories = ["Italian", "Egyptian", "Indian", "French", "Chinese"];
    public CreateRestaurantCommandValidator()
    {
        RuleFor(dto => dto.Name)
            .Length(3, 100);

        RuleFor(dto => dto.Description)
            .NotEmpty()
            .WithMessage("Description is required.");

        // Check for valid category using a predicate delegate instead of the custom method that is commented out.
        RuleFor(dto => dto.Category)
            .Must(category => Categories.Contains(category))
            .WithMessage("Invalid Category, Please choose one of the valid categories.");
            
            //.Custom((value, context) =>
            //{
            //    var isValidCategory = Categories.Contains(value);

            //    if (!isValidCategory)
            //    {
            //        context.AddFailure("Category", "Invalid Category, Please choose one of the valid categories.");
            //    }
            //});

        RuleFor(dto => dto.ContactEmail)
            .EmailAddress()
            .WithMessage("Provide a valid email address");

        RuleFor(dto => dto.PostalCode).Matches(@"^\d{2}-\d{3}$")
            .WithMessage("Provide a valid postal code (XX-XXX)");
    }
}
