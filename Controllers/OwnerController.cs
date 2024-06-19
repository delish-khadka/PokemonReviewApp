using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Identity.Client;
using PokemonReviewApp.DTO;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using PokemonReviewApp.Repository;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerController : Controller
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;

        public OwnerController(IOwnerRepository ownerRepository, IMapper mapper, ICountryRepository countryRepository)
        {
            _ownerRepository = ownerRepository;
            _mapper = mapper;
            _countryRepository = countryRepository;

        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
        public async Task<IActionResult> GetOwners()
        {
            var data = await _ownerRepository.GetOwners();
            var owners = _mapper.Map<List<OwnerDTO>>(data);
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            return Ok(owners);
        }

        [HttpGet("{ownerId}")]
        [ProducesResponseType(200, Type = typeof(Owner))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetOwner(int ownerId)
        {
            var ownerCheck =await _ownerRepository.OwnerExists(ownerId);
            if (!ownerCheck)
            {
                return NotFound();
            }
            var ownerData = await _ownerRepository.GetOwner(ownerId);
            var owner = _mapper.Map<OwnerDTO>(ownerData);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(owner);
        }

        [HttpGet("{ownerId}/pokemon")]
        [ProducesResponseType(200, Type = typeof(Owner))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetPokemonByOwner(int ownerId)
        {
            var ownerCheck =await _ownerRepository.OwnerExists(ownerId);

            if (!ownerCheck)
            {
                return NotFound();
            }
            var owner = _mapper.Map<List<PokemonDTO>>(await _ownerRepository.GetPokemonByOwner(ownerId));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(owner);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateOwner([FromQuery] int countryId, [FromBody] OwnerDTO ownerCreate)
        {
            if (ownerCreate == null)
            {
                return BadRequest(ModelState);
            }

            // Await the task to get the collection of owners
            var owners = await _ownerRepository.GetOwners();

            // Apply the LINQ query on the collection of owners
            var owner = owners
                .Where(c => c.LastName.Trim().ToUpper() == ownerCreate.LastName.TrimEnd().ToUpper())
                .FirstOrDefault();
            if (owner != null)
            {
                ModelState.AddModelError("", "Owner already exists");
                return StatusCode(422, ModelState);
            }
            var ownerMap = _mapper.Map<Owner>(ownerCreate);
            ownerMap.Country = await _countryRepository.GetCountry(countryId);
            // Call the asynchronous CreateOwner method and await its result
            var isCreated = await _ownerRepository.CreateOwner(ownerMap);

            if (!isCreated)
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully created owner");
        }

        //[HttpPut("{ownerId}")]
        [HttpPut]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateOwner([FromQuery] int ownerId, [FromBody] OwnerDTO updateOwner)
        {
            if (updateOwner == null)
            {
                return BadRequest(ModelState);
            }
            if (ownerId != updateOwner.Id)
            {
                return BadRequest(ModelState);
            }
            var ownerCheck = await _ownerRepository.OwnerExists(ownerId);

            if (!ownerCheck)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var ownerMap = _mapper.Map<Owner>(updateOwner);
            var mappedOwnerData = await _ownerRepository.UpdateOwner(ownerMap);
            if (!mappedOwnerData)
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [HttpDelete("{ownerId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteOwner(int ownerId)
        {
            var ownerCheck = await _ownerRepository.OwnerExists(ownerId);
            if(!ownerCheck)
                return NoContent();

            var ownerToDelete =await _ownerRepository.GetOwner(ownerId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var deleteOwner = await _ownerRepository.DeleteOwner(ownerToDelete);

            if (!deleteOwner)
            {
                ModelState.AddModelError("", "Something went wrong while deleting");
            }
            return NoContent();
        }
    }
}
