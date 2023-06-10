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

        public bool CreateOwner(Owner owner)
        {
                _context.Add(owner);
                return Save();
        }

        public Owner GetOwner(int ownerId)
        {
            return _context.Owners.Where(x=>x.Id == ownerId).FirstOrDefault();
        }

        public ICollection<Owner> GetOwnerOfAPokemon(int pokeId)
        {
            var owners = _context.PokemonOwners.Where(x=>x.Pokemon.Id == pokeId).Select(c=>c.Owner).ToList();
            return owners;
        }

        public ICollection<Owner> GetOwners()
        {
            return _context.Owners.ToList();
        }

        public ICollection<Pokemon> GetPokemonByOwner(int ownerId)
        {
            return _context.PokemonOwners.Where(x=>x.Owner.Id == ownerId).Select(x=>x.Pokemon).ToList();
        }

        public bool OwnerExists(int ownerId)
        {
            return _context.Owners.Any(x=>x.Id == ownerId);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}
