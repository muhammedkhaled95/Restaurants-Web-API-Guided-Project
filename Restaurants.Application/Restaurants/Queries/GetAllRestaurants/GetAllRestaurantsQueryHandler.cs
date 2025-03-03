using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Application.Common;
using Restaurants.Application.Restaurants.DTOs;
using Restaurants.Domain.Repositories;

namespace Restaurants.Application.Restaurants.Queries.GetAllRestaurants;

public class GetAllRestaurantsQueryHandler : IRequestHandler<GetAllRestaurantsQuery, PagedResults<RestaurantDto>>
{
    private readonly ILogger<GetAllRestaurantsQueryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IRestaurantsRepository _restaurantsRepository;
    public GetAllRestaurantsQueryHandler(IRestaurantsRepository restaurantsRepository, ILogger<GetAllRestaurantsQueryHandler> logger, IMapper mapper)
    {
        _restaurantsRepository = restaurantsRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResults<RestaurantDto>> Handle(GetAllRestaurantsQuery request, CancellationToken cancellationToken)
    {
        var searchPhraseLower = request.SearchPhrase?.ToLower();
        _logger.LogInformation("Getting All Restaurants");
        var (restaurants, totalCount) = await _restaurantsRepository.GetAllMatchingAsync(
            request.SearchPhrase,
            request.PageNumber,
            request.PageSize,
            request.SortBy,
            request.sortDirection);
        
        // Manual Mapping the returned restaurants entities to a Dto to be returned to the controller.
        //var restaurantsDto = restaurants.Select(restaurant => RestaurantDto.MapEntityToDto(restaurant));

        // Auto mapping the returned restaurants entities to a Dto to be returned to the controller.
        var restaurantsDto = _mapper.Map<IEnumerable<RestaurantDto>>(restaurants);
        var results = new PagedResults<RestaurantDto>(restaurantsDto, totalCount, request.PageSize, request.PageNumber);
        return results;
    }
}
