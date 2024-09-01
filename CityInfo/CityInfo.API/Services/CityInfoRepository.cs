using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace CityInfo.API.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private readonly CityInfoContext _context;

        public CityInfoRepository(CityInfoContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await _context.Cities.OrderBy(c => c.Name).ToListAsync();
        }
        public async Task<IEnumerable<City>> GetCitiesAsync(string? name, string? searchQuery, 
            int pageNumber, int pageSize)
        {
            //Collection to start from. Like this we can work on that collection.
            //So we can add 'where' clauses for filtering and searching.
            var collection = _context.Cities as IQueryable<City>;
            if (!string.IsNullOrEmpty(name))
            { 
                //We use trim to get rid of blank spaces.
                name = name.Trim();
                collection = collection.Where(c => c.Name == name); 
            }

            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(a => a.Name.Contains(searchQuery) || a.Description != null && 
                a.Description.Contains(searchQuery));   
            }
            return await collection.OrderBy(c => c.Name)
                //Its important to add paging functionallity last so we page the filter searched and ordered.
                //What we want to do is skip an amount of orders. pageSize times the requested pageNumber minus one
                //SO if page 2 is requested the amount of orders on the page 1 will be skipped and then we take the current requested pages.
                .Skip(pageSize * (pageNumber - 1)).Take(pageSize)
                .ToListAsync();           
        }
        public async Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest)
        {
            if (includePointsOfInterest)
            {
                return await _context.Cities.Include(c => c.PointsOfInterest)
                    .Where(c => c.Id == cityId).FirstOrDefaultAsync();
            }

            return await _context.Cities
                  .Where(c => c.Id == cityId).FirstOrDefaultAsync();
        }

        public async Task<bool> CityExistsAsync(int cityId)
        {
            return await _context.Cities.AnyAsync(c => c.Id == cityId);
        }

        public async Task<PointOfInterest?> GetPointOfInterestForCityAsync(
            int cityId,
            int pointOfInterestId)
        {
            return await _context.PointsOfInterest
               .Where(p => p.CityId == cityId && p.Id == pointOfInterestId)
               .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(
            int cityId)
        {
            return await _context.PointsOfInterest
                           .Where(p => p.CityId == cityId).ToListAsync();
        }

        public async Task AddPointOfInterestForCityAsync(int cityId,
            PointOfInterest pointOfInterest)
        {
            var city = await GetCityAsync(cityId, false);
            if (city != null)
            {
                //Doesn't go to the database yet
                city.PointsOfInterest.Add(pointOfInterest);
            }
        }
        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
        public void DeletePointOfInterest(PointOfInterest pointOfInterest)
        {
            _context.PointsOfInterest.Remove(pointOfInterest);
        }


    }
}
