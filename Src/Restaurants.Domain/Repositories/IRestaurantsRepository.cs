﻿using Restaurants.Domain.Constants;
using Restaurants.Domain.Entities;

namespace Restaurants.Domain.Repositories;

public interface IRestaurantsRepository
{
    Task<IEnumerable<Restaurant>> GetAllAsync();
    Task<(IEnumerable<Restaurant>, int)> GetAllMatchingAsync(string? SearchPhrase, int PageNumber, int PageSize, string? SortBy, SortDirection SortDirection);
    Task<Restaurant?> GetByIdAsync(int id);
    Task<int> Create(Restaurant restaurantEntity);
    Task Delete(Restaurant restaurantEntity);
    Task SaveChanges();
}
