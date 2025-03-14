﻿using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Restaurants.Application.Common;
using Restaurants.Domain.Constants;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Repositories;
using Restaurants.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace Restaurants.Infrastructure.Repositories;

internal class RestaurantsRepository(ApplicationDbContext dbContext) : IRestaurantsRepository
{
    public async Task<IEnumerable<Restaurant>> GetAllAsync()
    {
        var restaurants = await dbContext.Restaurants.ToListAsync();
        return restaurants;
    }

    public async Task<(IEnumerable<Restaurant>, int)> GetAllMatchingAsync(string? SearchPhrase, int PageNumber,
                                                                          int PageSize, string? SortBy, SortDirection SortDirection)
    {
        var searchPhraseLower = SearchPhrase?.ToLower();

        var baseQuery = dbContext.Restaurants.Where(r => searchPhraseLower == null ||
                (!string.IsNullOrEmpty(r.Name) && r.Name.Contains(searchPhraseLower))
                || (!string.IsNullOrEmpty(r.Description) && r.Description.ToLower().Contains(searchPhraseLower)));

        var totalCount = await baseQuery.CountAsync();

        if (SortBy != null)
        {
            // Create a dictionary that maps column names (as strings) to expressions (lambdas) that
            // know how to extract the values of those columns from a Restaurant object.
            // This is used to dynamically apply sorting based on the provided 'SortBy' parameter.
            var columnsSelector = new Dictionary<string, Expression<Func<Restaurant, Object>>>
            {
                // Map "Name" to a lambda that selects the Name property from a Restaurant.
                {nameof(Restaurant.Name), r => r.Name },

                // Map "Description" to a lambda that selects the Description property.
                {nameof(Restaurant.Description), r => r.Description },

                // Map "Category" to a lambda that selects the Category property.
                {nameof(Restaurant.Category), r => r.Category }
            };

            // Retrieve the expression (lambda) for the selected sorting column.
            // This will throw a KeyNotFoundException if 'SortBy' is not a valid key (you might want to add error handling).
            var selectedColumn = columnsSelector[SortBy];

            // Apply sorting to the query using the selected column and the desired sort direction.
            // If 'SortDirection' is Ascending, use OrderBy (ascending order).
            // If 'SortDirection' is Descending, use OrderByDescending (descending order).
            baseQuery = SortDirection == SortDirection.Ascending
                        ? baseQuery.OrderBy(selectedColumn)
                        : baseQuery.OrderByDescending(selectedColumn);
        }
        //Applying pagination with Skip and Take
        var restaurants = await baseQuery.Skip(PageSize * (PageNumber - 1))
                                   .Take(PageSize)
                                   .ToListAsync();

        return (restaurants, totalCount);
    }

    public async Task<Restaurant?> GetByIdAsync(int id)
    {
        var restaurant = await dbContext.Restaurants.Include(restaurant => restaurant.dishes).FirstOrDefaultAsync(x => x.Id == id);
        return restaurant;
    }

    public async Task<int> Create(Restaurant restaurantEntity)
    {
        /*
         * Why dbContext.Add is not asynchronous
         * =====================================
         * The dbContext.Add method in Entity Framework Core is not an asynchronous operation because it does not interact 
         * with the database when called. Instead, it only marks the entity (restaurantEntity in this case)
         * as being in the "Added" state in the DbContext's change tracker.
         * The actual interaction with the database happens when you call dbContext.SaveChangesAsync(). 
         * At that point, EF Core translates the changes into SQL commands and executes them against the database.
         */
        dbContext.Add(restaurantEntity);
        await dbContext.SaveChangesAsync();

        /*
         * How we got the Id from restaurantEntity 
         * =======================================
         * When you save an entity to the database using EF Core (via SaveChanges or SaveChangesAsync),
         * the database generates the Id if it's configured as an identity column (e.g., an auto-increment primary key).
         *                            ----------------------------------------------
         * Here’s what happens in detail: 
         * ==============================
         * Before SaveChangesAsync: The Id property of restaurantEntity is likely 0 (default for integers) or 
         * whatever value you set explicitly (if any).
         * During SaveChangesAsync: 
         * EF Core sends an INSERT statement to the database.
         * If the Id column in the database is an identity column, the database generates the new ID value.
         * After SaveChangesAsync:
         * EF Core retrieves the generated Id value from the database and updates the restaurantEntity object in memory with the new Id.
         */
        return restaurantEntity.Id;
    }

    public async Task Delete(Restaurant restaurantEntity)
    {
        dbContext.Remove(restaurantEntity);
        await dbContext.SaveChangesAsync();
    }

    public async Task SaveChanges()
    {
        await dbContext.SaveChangesAsync();
    }
}