using AutoMapper;
using Restaurants.Application.Restaurants.Commands.CreateRestaurants;
using Restaurants.Application.Restaurants.Commands.UpdateRestaurants;
using Restaurants.Domain.Entities;

namespace Restaurants.Application.Restaurants.DTOs;

public class RestaurantsProfile : Profile
{
    public RestaurantsProfile()
    {
        // This map is used to map from the update restaurant command request data to the restaurant entity.
        CreateMap<UpdateRestaurantCommand, Restaurant>();
        // This is used when we use CQRS to map between the command data and the entity.
        CreateMap<CreateRestaurantCommand, Restaurant>()
        .ForMember(d => d.Address, opt => opt.MapFrom(src =>
            new Address
            {
                City = src.City,
                PostalCode = src.PostalCode,
                Street = src.Street,
            }));


        CreateMap<Restaurant, RestaurantDto>()
        .ForMember(d => d.City,  opt => opt.MapFrom(src => src.Address == null ? null : src.Address.City))
        .ForMember(d => d.PostalCode, opt => opt.MapFrom(src => src.Address == null ? null : src.Address.PostalCode))
        .ForMember(d => d.Street, opt => opt.MapFrom(src => src.Address == null ? null : src.Address.Street))
        .ForMember(d => d.dishes, opt => opt.MapFrom(src => src.dishes == null ? null : src.dishes));
    }
}
