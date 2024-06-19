using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using PokemonReviewApp.DTO;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
        public async Task<IActionResult> GetCategories()
        {
            var categoryData = await _categoryRepository.GetCategories();
            var categories =  _mapper.Map<List<CategoryDTO>>(categoryData);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(categories);
        }


        [HttpGet("{categoryId}")]
        [ProducesResponseType(200, Type = typeof(Category))]
        public async Task<IActionResult> GetCategory(int categoryId)
        {
            var categoryCheck = await _categoryRepository.CategoryExists(categoryId);
            if (!categoryCheck)
            {
                return NotFound();
            }
            var category = _mapper.Map<CategoryDTO>(await _categoryRepository.GetCategory(categoryId));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(category);
        }


        [HttpGet("pokemon/{categoryId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetPokemonByCategoryId(int categoryId)
        {
            var pokemons = _mapper.Map<List<PokemonDTO>>(
                await _categoryRepository.GetPokemonByCategory(categoryId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(pokemons);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDTO categoryCreate)
        {
            if (categoryCreate == null)
            {
                return BadRequest(ModelState);
            }
            var categories = await _categoryRepository.GetCategories();
            var category = categories
                .Where(c => c.Name.Trim().ToUpper() == categoryCreate.Name.TrimEnd().ToUpper())
                .FirstOrDefault();

            if (category != null)
            {
                ModelState.AddModelError("", "Category already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var categoryMap = _mapper.Map<Category>(categoryCreate);
            if (!await _categoryRepository.CreateCategory(categoryMap)) {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully created");
        }

        [HttpPut("{categoryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateCategory(int categoryId, [FromBody] CategoryDTO updatedCategory) {
            if (updatedCategory == null)
            {
                return BadRequest(ModelState);
            }
            if (categoryId != updatedCategory.Id)
            {
                return BadRequest(ModelState);
            }
            if (!await _categoryRepository.CategoryExists(categoryId))
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var categoryMap = _mapper.Map<Category>(updatedCategory);
            if (!await _categoryRepository.UpdateCategory(categoryMap))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [HttpDelete("{categoryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            if (!await _categoryRepository.CategoryExists(categoryId))
            {
                return NotFound();
            }
             
            var categoryToDelete = await _categoryRepository.GetCategory(categoryId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!await _categoryRepository.DeleteCategory(categoryToDelete))
            {
                ModelState.AddModelError("", "Something went wrong while deleting");
            }

            return NoContent();
        }

    }
}
