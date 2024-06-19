using Microsoft.EntityFrameworkCore;
using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class PokemonRepository: IPokemonRepository
    {
        private readonly ApplicationDbContext _context;
        public PokemonRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreatePokemon(int ownerId, int categoryId, Pokemon pokemon)
        {
            var pokemonOwnerEntity = await _context.Owners.Where(a => a.Id == ownerId).FirstOrDefaultAsync();
            var category = await _context.Categories.Where(a => a.Id == categoryId).FirstOrDefaultAsync();

            var pokemonOwner = new PokemonOwner()
            {
                Owner = pokemonOwnerEntity,
                Pokemon = pokemon,
            };

            await _context.AddAsync(pokemonOwner);

            var pokemonCategory = new PokemonCategory()
            {
                Category = category,
                Pokemon = pokemon,
            };
            await _context.AddAsync(pokemonCategory);

            await _context.AddAsync(pokemon);
            return await Save();
        }

        public async Task<bool> DeletePokemon(Pokemon pokemon)
        {
            _context.Remove(pokemon);
            return await Save();
        }

        public async Task<Pokemon> GetPokemon(int id)
        {

            return await _context.Pokemons
                .Include(p => p.PokemonCategories)
                .ThenInclude(pc => pc.Category) // Ensure Category is loaded
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Pokemon> GetPokemon(string name)
        {
            return await _context.Pokemons.Where(p => p.Name == name).FirstOrDefaultAsync();
        }

        public async Task<decimal> GetPokemonRating(int pokeId)
        {
            var review = await _context.Reviews.Where(p => p.Pokemon.Id == pokeId).ToListAsync();
            if(review.Count() <= 0)
            {
                return 0;
            }
            return ((decimal)review.Sum(r => r.Rating) / review.Count());
        }

        public async Task<ICollection<Pokemon>> GetPokemons()
        {
            return await _context.Pokemons
                .Include(p => p.PokemonCategories)
                .ThenInclude(pc => pc.Category) 
                .OrderBy(p=>p.Id)
                .ToListAsync();
        }

        public async Task<bool> PokemonExists(int pokeId)
        {
            return await _context.Pokemons.AnyAsync(x => x.Id == pokeId);
        }

        public async Task<bool> Save()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }

        public async Task<bool> UpdatePokemon(int ownerId, int categoryId, Pokemon pokemon)
        {
            var pokemonOwnerEntity = await _context.PokemonOwners.
                Where(o => o.PokemonId == pokemon.Id).FirstOrDefaultAsync();

            var OwnerEntity = await _context.Owners.
                Where(o => o.Id == ownerId).FirstOrDefaultAsync();

            var pokemonCategoryEntity = await _context.PokemonCategories.
                Where(c => c.PokemonId == pokemon.Id).FirstOrDefaultAsync();

            var categoryEntity =  await _context.Categories.Where(o => o.Id == categoryId).FirstOrDefaultAsync();

            if (pokemonOwnerEntity != null && OwnerEntity != null)
            {

                _context.Remove(pokemonOwnerEntity);

                var pokemonOwner = new PokemonOwner()
                {
                    Owner = OwnerEntity,
                    Pokemon = pokemon,
                };
                await _context.AddAsync(pokemonOwner);
            }

            if (pokemonCategoryEntity != null && categoryEntity != null)
            {

                _context.Remove(pokemonCategoryEntity);

                var pokemonCategory = new PokemonCategory()
                {
                    Category = categoryEntity,
                    Pokemon = pokemon,
                };
                await _context.AddAsync(pokemonCategory);
            }
            else
                return false;


            _context.Update(pokemon);
            return await Save();
        }
    }
}
