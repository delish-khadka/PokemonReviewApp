using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PokemonReviewApp.DTO;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using PokemonReviewApp.Repository;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonController : Controller
    {
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IOwnerRepository _ownerRepository;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;
        public PokemonController(IPokemonRepository pokemonRepository, IMapper mapper, ICategoryRepository categoryRepository
            , IOwnerRepository ownerRepository, IReviewRepository reviewRepository, Cloudinary cloudinary)
        {
            _pokemonRepository = pokemonRepository;
            _mapper = mapper;
            _categoryRepository = categoryRepository;
            _ownerRepository = ownerRepository;
            _reviewRepository = reviewRepository;
            _cloudinary = cloudinary;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
        public async Task<IActionResult> GetPokemons()
        {
            var pokemons = _mapper.Map<List<PokemonDTO>>(await _pokemonRepository.GetPokemons());
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(pokemons);
        }

        [HttpGet("{pokeId}")]
        [ProducesResponseType(200, Type = typeof(PokemonDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetPokemon(int pokeId)
        {
            if (!await _pokemonRepository.PokemonExists(pokeId))
            {
                return NotFound();
            }
            var repdata = await _pokemonRepository.GetPokemon(pokeId);
            var pokemon = _mapper.Map<PokemonDTO>(repdata);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(pokemon);
        }

        [HttpGet("{pokeId}/rating")]
        [ProducesResponseType(200, Type = typeof(decimal))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetPokemonRating(int pokeId)
        {
            if (!await _pokemonRepository.PokemonExists(pokeId))
            {
                return NotFound();
            }
            var rating = await _pokemonRepository.GetPokemonRating(pokeId);
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            return Ok(rating);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreatePokemon(
            [FromForm]IFormFile image,
            [FromForm] int ownerId, 
            [FromForm] int categoryId, 
            [FromBody] PokemonDTO pokemonCreate)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
            if (pokemonCreate == null)
            {
                return BadRequest(ModelState);
            }
            var pokeData = await _pokemonRepository.GetPokemons();
            var pokemons = pokeData.
                Where(p => p.Name.Trim().ToUpper() == pokemonCreate.Name.TrimEnd().ToUpper())
                .FirstOrDefault();
            if (pokemons != null)
            {
                ModelState.AddModelError("", "Pokemon already exists");
                return StatusCode(422, ModelState);
            }
            var pokemonMap = _mapper.Map<Pokemon>(pokemonCreate);

            if (! await _pokemonRepository.CreatePokemon(ownerId, categoryId, pokemonMap))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            using (var stream = image.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(image.FileName, stream)
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            }
            return Ok("Successfully Created");

        }

        [HttpPut("{pokeId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdatePokemon(int pokeId,
            [FromQuery] int ownerId, 
            [FromQuery] int categoryId,
            [FromBody] PokemonDTO updatePokemon)
        {
            if (updatePokemon == null)
            {
                return BadRequest(ModelState);
            }
            if (pokeId != updatePokemon.Id)
            {
                return BadRequest(ModelState);
            }
            if (!await _pokemonRepository.PokemonExists(pokeId))
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var pokemonMap = _mapper.Map<Pokemon>(updatePokemon);
            if (!await _pokemonRepository.UpdatePokemon(ownerId, categoryId, pokemonMap))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [HttpDelete("{pokeId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeletePokemon(int pokeId) {
            if (!await _pokemonRepository.PokemonExists(pokeId))
                return NotFound();

            var reviewsToDelete = await _reviewRepository.GetReviewsOfAPokemon(pokeId);
            var pokemonToDelete = await _pokemonRepository.GetPokemon(pokeId);
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (reviewsToDelete.Count > 0)
            {
                if (!await _reviewRepository.DeleteReviews(reviewsToDelete.ToList()))
                {
                    ModelState.AddModelError("", "Something went wrong while deleting reviews");

                }
            }
            
            if (!await _pokemonRepository.DeletePokemon(pokemonToDelete))
            {
                ModelState.AddModelError("", "Something went wrong while deleting");
            }
            return NoContent();
        }
    }
}
