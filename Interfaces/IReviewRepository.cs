using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interfaces
{
    public interface IReviewRepository
    {
        Task<ICollection<Review>> GetReviews();
        Task<Review> GetReview(int reviewId);
        Task<ICollection<Review>> GetReviewsOfAPokemon(int pokeId);
        Task<bool> ReviewExists(int reviewId);
        Task<bool> CreateReview(Review review);
        Task<bool> Save();
        Task<bool> UpdateReview(Review review);
        Task<bool> DeleteReview(Review review);
        Task<bool> DeleteReviews(List<Review> reviews);
    }
}
