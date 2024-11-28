using FluentValidation;
using Restaurants.Application.Restaurants.DTOs;

namespace Restaurants.Application.Restaurants.Validators;

public class CreateRestaurantDtoValidator : AbstractValidator<CreateRestaurantDto>
{
    public CreateRestaurantDtoValidator()
    {
        RuleFor(dto => dto.Name)
            .Length(3, 100);

        RuleFor(dto => dto.Description)
            .NotEmpty()
            .WithMessage("Description is required.");

        RuleFor(dto => dto.Category)
            .NotEmpty()
            .WithMessage("Category is required.");

        RuleFor(dto => dto.ContactEmail)
            .EmailAddress()
            .WithMessage("Provide a valid email address");

        RuleFor(dto => dto.PostalCode).Matches(@"^\d{2}-\d{3}$")
            .WithMessage("Provide a valid postal code (XX-XXX)");
    }
}
