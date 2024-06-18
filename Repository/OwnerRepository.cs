using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class OwnerRepository : IOwnerRepository
    {
        private readonly ApplicationDbContext _context;
        public OwnerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateOwner(Owner owner)
        {
                await _context.AddAsync(owner);
                return await Save();
        }

        public async Task<bool> DeleteOwner(Owner owner)
        {
            _context.Remove(owner);
            return await Save();
        }

        public async Task<Owner> GetOwner(int ownerId)
        {
            return await _context.Owners.Where(x=>x.Id == ownerId).FirstOrDefaultAsync();
        }

        public async Task<ICollection<Owner>> GetOwnerOfAPokemon(int pokeId)
        {
            var owners = await _context.PokemonOwners.Where(x=>x.Pokemon.Id == pokeId).Select(c=>c.Owner).ToListAsync();
            return owners;
        }

        public async Task<ICollection<Owner>> GetOwners()
        {
            return await _context.Owners.ToListAsync();
        }

        public async Task<ICollection<Pokemon>> GetPokemonByOwner(int ownerId)
        {
            return await _context.PokemonOwners.Where(x=>x.Owner.Id == ownerId).Select(x=>x.Pokemon).ToListAsync();
        }

        public async Task<bool> OwnerExists(int ownerId)
        {
            return await _context.Owners.AnyAsync(x=>x.Id == ownerId);
        }

        public async Task<bool> Save()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }

        public async Task<bool> UpdateOwner(Owner owner)
        {
            _context.Update(owner);
            return await Save();
        }
    }
}
