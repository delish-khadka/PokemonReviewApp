using Microsoft.EntityFrameworkCore;
using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class CountryRepository : ICountryRepository
    {
        private readonly ApplicationDbContext _context;
        public CountryRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> CountryExists(int id)
        {
            return await _context.Countries.AnyAsync(c => c.Id == id);
        }

        public async Task<bool> CreateCountry(Country country)
        {
            _context.Add(country);
            return await Save();
        }

        public async Task<bool> DeleteCountry(Country country)
        {
            _context.Remove(country);
            return await Save();
        }

        public async Task<ICollection<Country>> GetCountries()
        {
            return await _context.Countries.ToListAsync();
        }

        public async Task<Country> GetCountry(int id)
        {
            return await _context.Countries.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task< Country> GetCountryByOwner(int ownerId)
        {
            return await _context.Owners.Where(x => x.Id == ownerId).Select(c => c.Country).FirstOrDefaultAsync();
        }

        public async Task<ICollection<Owner>> GetOwnersFromACountry(int countryId)
        {
            return await _context.Owners.Where(c => c.Country.Id == countryId).ToListAsync();
        }

        public async Task<bool> Save()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }

        public async Task<bool> UpdateCountry(Country country)
        {
            _context.Update(country);
            return await Save();
        }
    }
}
