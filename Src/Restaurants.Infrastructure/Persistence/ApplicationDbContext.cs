using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Restaurants.Domain.Entities;
using System;

namespace Restaurants.Infrastructure.Persistence
{
    internal class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) // Pass options to the base DbContext class constructor
        {

        }

        internal DbSet<Restaurant> Restaurants { get; set; }
        internal DbSet<Dish> Dishes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Restaurant>().OwnsOne(r => r.Address);

            modelBuilder.Entity<Restaurant>().HasMany(r => r.dishes)
                                             .WithOne()
                                             .HasForeignKey(d => d.RestaurantId)
                                             .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>().HasMany(u => u.OwnedRestaurants)
                                       .WithOne(r => r.Owner)
                                       .HasForeignKey(r => r.OwnerId)
                                       .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
