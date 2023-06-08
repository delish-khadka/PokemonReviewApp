﻿using AutoMapper;
using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext _context;
        private IMapper _mapper;
        public ReviewRepository(ApplicationDbContext context, Mapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public Review GetReview(int reviewId)
        {
            return _context.Reviews.Where(x => x.Id == reviewId).FirstOrDefault();
        }

        public ICollection<Review> GetReviews()
        {
            return _context.Reviews.ToList();
        }

        public ICollection<Review> GetReviewsOfAPokemon(int pokeId)
        {
            return _context.Reviews.Where(r=>r.Pokemon.Id == pokeId).ToList();
        }

        public bool ReviewExists(int reviewId)
        {
            return _context.Reviews.Any(r=>r.Id == reviewId);
        }
    }
}
