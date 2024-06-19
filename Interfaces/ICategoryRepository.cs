using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interfaces
{
    public interface ICategoryRepository
    {
        Task<ICollection<Category>> GetCategories();
        Task<Category> GetCategory(int id);
        Task<ICollection<Pokemon>> GetPokemonByCategory(int categoryId);
        Task<bool> CategoryExists(int id);
        Task<bool> CreateCategory(Category category);
        Task<bool> Save();
        Task<bool> UpdateCategory(Category category);
        Task<bool> DeleteCategory(Category category);
    }
}
