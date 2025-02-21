using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Restaurants.Application.Restaurants;
using Restaurants.Application.User;

namespace Restaurants.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        //This line of code is used to register AutoMapper in the dependency injection (DI) container 
        //and configure it to automatically scan for mapping profiles in a specified assembly.

        /*
         * The project with the ServiceCollectionExtensions class (and the mapping profiles) is a class library project which is the application layer
         * here in our project. 
         * When you reference this project from your main project:
         * Its assembly (YourClassLibrary.dll) is loaded at runtime.
         * AutoMapper scans that assembly for mapping profiles and registers them in the DI container.
         */
        var applicationAssembly = typeof(ServiceCollectionExtensions).Assembly;

        services.AddAutoMapper(applicationAssembly);

        // Adding the fluent validation service the same way as we did with the auto mapper service above.
        services.AddValidatorsFromAssembly(applicationAssembly)
                .AddFluentValidationAutoValidation();

        // Register the Mediatr Service.
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly));

        // 1️- services.AddScoped<IUserContext, UserContext>();
        //    - Registers UserContext as the implementation for IUserContext with a scoped lifetime.
        //    - Scoped: A new instance is created per HTTP request, ensuring request-specific data like the current user is consistent.
        services.AddScoped<IUserContext, UserContext>();

        // 2️- services.AddHttpContextAccessor();
        //    - Registers IHttpContextAccessor to access the current HttpContext in services and other non-controller classes.
        //    - Enables retrieval of user claims, headers, and request info outside controllers.
        services.AddHttpContextAccessor();

    }
}

