using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace CityInfo.API.DbContexts
{
    public class CityInfoContext : DbContext
    {
       
        public DbSet<City> Cities { get; set; } 
        public DbSet<PointOfInterest> PointsOfInterest { get; set; }

        public CityInfoContext(DbContextOptions<CityInfoContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>().HasData(
                new City("New York City")
                {
                    Id = 1,
                    Description = "The one with that big park."
                },
                new City("Antwerp")
                {
                    Id = 2,
                    Description = "The one with the cathedral that was never really finished."
                },
                new City("London")
                {
                    Id = 3,
                    Description = "The one with that big watch."
                });
            modelBuilder.Entity<PointOfInterest>().HasData(
                new PointOfInterest("Central Park")
                {
                    Id = 1,
                    CityId = 1,
                    Description = "The most visited urban park in the USA."
                },
                new PointOfInterest("Empire State Building")
                {
                    Id = 2,
                    CityId = 1,
                    Description = "A 102-story skycraper located in Midtown Manhattan."
                },
                new PointOfInterest("Cathedral")
                {
                    Id = 3,
                    CityId = 2,
                    Description = "A gothic style cathedral, conceived by architects Jan and Pieter Appelmans."
                },
                new PointOfInterest("Central Station")
                {
                    Id = 4,
                    CityId = 2,
                    Description = "The finest example of railway architecture in Belgium."
                },
                new PointOfInterest("British Museum")
                {
                    Id = 5,
                    CityId = 3,
                    Description = "One of the most visited museums of the world"
                },
                new PointOfInterest ("Windsor Palace")
                {
                    Id = 6,
                    CityId = 3,
                    Description = "Place of residence of the british royal family."
                }
                );
            base.OnModelCreating(modelBuilder);
        }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlite("connectionstring");
        //    base.OnConfiguring(optionsBuilder);
        //}



    }
}
