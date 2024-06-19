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
    public class CountryController : Controller
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;
        public CountryController(ICountryRepository countryRepository, IMapper mapper)
        {
            _countryRepository = countryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Country>))]
        public async Task<IActionResult> GetCountries()
        {
            var countries = _mapper.Map<List<CountryDTO>>(await _countryRepository.GetCountries());
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            return Ok(countries);
        }

        [HttpGet("{countryId}")]
        [ProducesResponseType(200, Type = typeof(Country))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetCountry(int countryId)
        {
            if (!await _countryRepository.CountryExists(countryId))
            {
                return NotFound();
            }
            var country = _mapper.Map<CountryDTO>(await _countryRepository.GetCountry(countryId));
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            return Ok(country);
        }

        [HttpGet("/owners/{ownerId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200,Type = typeof(Country))]
        public async Task<IActionResult> GetCountryOfAnOwner(int ownerId)
        {
            var country = _mapper.Map<CountryDTO>(
                await _countryRepository.GetCountryByOwner(ownerId));

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            return Ok(country);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateCountry([FromBody] CountryDTO countryCreate)
        {
            if(countryCreate == null)
            {
                return BadRequest(ModelState);
            }
            var countries = await _countryRepository.GetCountries();
            var country = countries.
                Where(c => c.Name.Trim().ToUpper() == countryCreate.Name.TrimEnd().ToUpper())
                .FirstOrDefault();
            if(country != null)
            {
                ModelState.AddModelError("", "Country already exists");
                return StatusCode(422, ModelState);
            }
            var countryMap = _mapper.Map<Country>(countryCreate);
            if (!await _countryRepository.CreateCountry(countryMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully created country");
        }

        [HttpPut("{countryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateCategory(int countryId, [FromBody] CountryDTO updateCountry)
        {
            if (updateCountry == null)
            {
                return BadRequest(ModelState);
            }
            if (countryId != updateCountry.Id)
            {
                return BadRequest(ModelState);
            }
            if (!await _countryRepository.CountryExists(countryId))
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var countryMap = _mapper.Map<Country>(updateCountry);
            if (!await _countryRepository.UpdateCountry(countryMap))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }


        [HttpDelete("{countryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteCountry(int countryId)
        {
            if (!await _countryRepository.CountryExists(countryId))
            {
                return NotFound();
            }
            var countryToDelete = await _countryRepository.GetCountry(countryId);

            if(!ModelState.IsValid)
                return NotFound();

            if(!await _countryRepository.DeleteCountry(countryToDelete))
            {
                ModelState.AddModelError("", "Something went wrong while deleting");
            }
            return NoContent();
        }
    }
}
