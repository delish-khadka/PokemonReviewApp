using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext _context;
        
        public ReviewRepository(ApplicationDbContext context)
        {
            _context = context;
            
        }

        public async Task<bool> CreateReview(Review review)
        {
            _context.Add(review);
            return await Save();
        }

        public async Task<bool> DeleteReview(Review review)
        {
            _context.Remove(review);
            return await Save();
        }

        public async Task<bool> DeleteReviews(List<Review> reviews)
        {
            _context.RemoveRange(reviews);
            return await Save();

        }

        public async Task<Review> GetReview(int reviewId)
        {
            return await _context.Reviews.Where(x => x.Id == reviewId).FirstOrDefaultAsync();
        }

        public async Task<ICollection<Review>> GetReviews()
        {
            return await _context.Reviews.ToListAsync();
        }

        public async Task<ICollection<Review>> GetReviewsOfAPokemon(int pokeId)
        {
            return await _context.Reviews.Where(r=>r.Pokemon.Id == pokeId).ToListAsync();
        }

        public async Task<bool> ReviewExists(int reviewId)
        {
            return await _context.Reviews.AnyAsync(r=>r.Id == reviewId);
        }

        public async Task<bool> Save()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0?true :false;
        }

        public async Task<bool> UpdateReview(Review review)
        {
            _context.Update(review);
            return await Save();
        }
    }
}
