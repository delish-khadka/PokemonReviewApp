using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class ReviewerRepository : IReviewerRepository
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        public ReviewerRepository(ApplicationDbContext context, IMapper mapper) { 
            _mapper = mapper;
            _context = context;
        }

        public async Task<bool> CreateReviewer(Reviewer reviewer)
        {
            await _context.AddAsync(reviewer);
            return await Save();
        }

        public async Task<bool> DeleteReviewer(Reviewer reviewer)
        {
            _context.Remove(reviewer);
            return await Save();
        }

        public async Task<Reviewer> GetReviewer(int reviewerId)
        {
            return await _context.Reviewers.Where(x => x.Id == reviewerId).Include(e=>e.Review).FirstOrDefaultAsync();
        }

        public async Task<ICollection<Reviewer>> GetReviewers()
        {
            return await _context.Reviewers.ToListAsync();
        }

        public async Task<ICollection<Review>> GetReviewsByReviewer(int reviewerId)
        {
            return await _context.Reviews.Where(x=>x.Reviewer.Id == reviewerId).ToListAsync();
        }

        public async Task<bool> ReviewerExists(int reviewerId)
        {
            return await _context.Reviewers.AnyAsync(r => r.Id == reviewerId);
        }

        public async Task<bool> Save()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }

        public async Task<bool> UpdateReviewer(Reviewer reviewer)
        {
            _context.Update(reviewer);
            return await Save();
        }
    }
}
