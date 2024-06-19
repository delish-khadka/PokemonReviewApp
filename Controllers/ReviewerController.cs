using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.DTO;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using PokemonReviewApp.Repository;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewerController : Controller
    {
        
        private readonly IReviewerRepository _reviewerRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;

        public ReviewerController(IReviewerRepository reviewerRepository,IMapper mapper, IReviewRepository reviewRepository)
        {
            _mapper = mapper;
            _reviewerRepository = reviewerRepository;
            _reviewRepository = reviewRepository;

        }

        [HttpGet]
        [ProducesResponseType(200, Type =typeof(IEnumerable<Reviewer>))]
        public async Task<IActionResult> GetReviewers() {
            var pokemons = _mapper.Map<List<ReviewerDTO>>(await _reviewerRepository.GetReviewers());
            if (!ModelState.IsValid) { 
                return BadRequest(ModelState);
            }
            return Ok(pokemons);
        }

        [HttpGet("{reviewerId}")]
        [ProducesResponseType(200,Type = typeof(Reviewer))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetPokemon(int reviewerId)
        {
            if(!await _reviewerRepository.ReviewerExists(reviewerId))
            {
                return NotFound();
            }
            var reviewer = _mapper.Map<ReviewerDTO>(await _reviewerRepository.GetReviewer(reviewerId));
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(reviewer);
        }

        [HttpGet("{reviewerId}/reviews")]
        public async Task<IActionResult> GetReviewsByAReviewer(int reviewerId)
        {
            if (!await _reviewerRepository.ReviewerExists(reviewerId))
            {
                return NotFound();
            }
            var reviews = _mapper.Map<List<ReviewDTO>>(await _reviewerRepository.GetReviewsByReviewer(reviewerId));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(reviews);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateReviewer([FromQuery] int reviewId, [FromBody] ReviewerDTO reviewerCreate)
        {
            if (reviewerCreate == null)
            {
                return BadRequest(ModelState);
            }

            /*var reviewer = _reviewerRepository.GetReviewers().
                Where(p => p.LastName.Trim().ToUpper() == reviewCreate.LastName.TrimEnd().ToUpper())
                .FirstOrDefault();
            if (reviewer != null)
            {
                ModelState.AddModelError("", "Reviewer already exists");
                return StatusCode(422, ModelState);
            }*/
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var reviewerMap = _mapper.Map<Reviewer>(reviewerCreate);

            if (!await _reviewerRepository.CreateReviewer(reviewerMap))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully Created");

        }

        [HttpPut("{reviewerId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateCategory(int reviewerId, [FromBody] ReviewerDTO updatedReviewer)
        {
            if (updatedReviewer == null)
            {
                return BadRequest(ModelState);
            }
            if (reviewerId != updatedReviewer.Id)
            {
                return BadRequest(ModelState);
            }
            if (!await _reviewerRepository.ReviewerExists(reviewerId))
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var reviewerMap = _mapper.Map<Reviewer>(updatedReviewer);
            if (!await _reviewerRepository.UpdateReviewer(reviewerMap))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [HttpDelete("{reviewerId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteReviewer(int reviewerId)
        {
            if (!await _reviewerRepository.ReviewerExists(reviewerId))
                return NoContent();

            var reviewerToDelete =await _reviewerRepository.GetReviewer(reviewerId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await _reviewerRepository.DeleteReviewer(reviewerToDelete))
            {
                ModelState.AddModelError("", "Something went wrong while deleting");
            }
            return NoContent();
        }
    }
}
