using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using CityInfo.API.Stores;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {
        private readonly ICityInfoRepository _cityInforRepository;

        private readonly IMapper _mapper;

        public CitiesController(ICityInfoRepository cityInforRepository, IMapper mapper)
        {
            _cityInforRepository = cityInforRepository ?? throw new ArgumentNullException(nameof(cityInforRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPointOfInterestDto>>> GetCities()
        {
            var cityEntities = await _cityInforRepository.GetCitiesAsync();

            return Ok(_mapper.Map<IEnumerable<CityWithoutPointOfInterestDto>>(cityEntities));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCity(int id, bool includePointsOfInterest)
        {
            var cityEntity = await _cityInforRepository.GetCityAsync(id, includePointsOfInterest);

            if (cityEntity == null) return NotFound();

            if (includePointsOfInterest) return Ok(_mapper.Map<CityDto>(cityEntity));

            return Ok(_mapper.Map<CityWithoutPointOfInterestDto>(cityEntity));
        }
    }
}
