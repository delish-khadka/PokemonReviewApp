using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interfaces
{
    public interface IPokemonRepository
    {
        Task<ICollection<Pokemon>> GetPokemons();
        Task<Pokemon> GetPokemon(int id);
        Task<Pokemon> GetPokemon(string name);
        Task<decimal> GetPokemonRating(int pokeId);
        Task<bool> PokemonExists(int pokeId);
        Task<bool> CreatePokemon(int ownerId, int categoryId, Pokemon pokemon);
        Task<bool> DeletePokemon(Pokemon pokemon);
        Task<bool> Save();
        Task<bool> UpdatePokemon(int ownerId,int categoryId, Pokemon pokemon);
    }
}
