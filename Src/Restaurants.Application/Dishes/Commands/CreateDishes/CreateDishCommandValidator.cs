using FluentValidation;

namespace Restaurants.Application.Dishes.Commands.CreateDishes;

public class CreateDishCommandValidator : AbstractValidator<CreateDishCommand>
{
    public CreateDishCommandValidator()
    {
        RuleFor(dish => dish.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Price can not be a negative number");

        RuleFor(dish => dish.KiloCalories)
           .GreaterThanOrEqualTo(0)
           .WithMessage("KiloCalories can not be a negative number");
    }
}
