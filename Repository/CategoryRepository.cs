using Microsoft.EntityFrameworkCore;
using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> CategoryExists(int id)
        {
            return await _context.Categories.AnyAsync(x => x.Id == id);
        }

        public async Task<bool> CreateCategory(Category category)
        {
            await _context.AddAsync(category);
            return await Save();
        }

        public async Task<bool> DeleteCategory(Category category)
        {
            _context.Remove(category);
            return await Save();
        }

        public async Task<ICollection<Category>> GetCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category> GetCategory(int id)
        {
            return await _context.Categories.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<ICollection<Pokemon>> GetPokemonByCategory(int categoryId)
        {
           return await _context.PokemonCategories.Where(x=>x.CategoryId == categoryId).Select(x => x.Pokemon).ToListAsync();
        }

        public async Task<bool> Save()
        {
            var saved = await  _context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }

        public async Task<bool> UpdateCategory(Category category)
        {
            _context.Update(category);
            return await Save();
        }
    }
}
