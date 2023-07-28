using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Authorize]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version.apiVersion}/cities/{cityId}/pointsofinterest")]
    public class PointsOfInterestController : ControllerBase
    {
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly IMailService _mailService;
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, 
            IMailService mailService,
            ICityInfoRepository cityInfoRepository,
            IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");

                return NotFound();
            }

            var pointOfInterestForCity = await _cityInfoRepository.GetPointsOfInterestForCityAsync(cityId);

            return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointOfInterestForCity));
            //try
            //{
            //    // throw new Exception("Exception Sample");

            //    var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

            //    if (city == null)
            //    {
            //        _logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest");
            //        return NotFound();
            //    }

            //    return Ok(city.PointsOfInterest);
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogCritical($"Exception while getting points of interest for city with id {cityId}.", ex);
            //    return StatusCode(500, "A problem occured while handling your request.");
            //}
            
        }

        [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
        public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(int cityId, int pointOfInterestId)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId)) return NotFound();

            var pointOfInterest = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if (pointOfInterest == null) return NotFound();

            return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));
        }

        [HttpPost]
        public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(int cityId, 
            PointOfInterestForCreationDto pointOfInterest)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId)) return NotFound();

            var finalPointOfInterest = _mapper.Map<Entities.PointOfInterest>(pointOfInterest);

            await _cityInfoRepository.AddPointOfInterestForCityAsync(cityId, finalPointOfInterest);

            await _cityInfoRepository.SafeChangesAsync();

            var createdPointOfInterestToReturn = _mapper.Map<Models.PointOfInterestDto>(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest",
                new
                {
                    cityId = cityId,
                    pointOfInterestId = createdPointOfInterestToReturn.Id
                },
                createdPointOfInterestToReturn);
        }

        [HttpPut("{pointofinterestid}")]
        public async Task<ActionResult> UpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterest)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId)) return NotFound();

            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if (pointOfInterestEntity == null) return NotFound();

            _mapper.Map(pointOfInterest, pointOfInterestEntity);
            await _cityInfoRepository.SafeChangesAsync();

            return NoContent();
        }

        [HttpPatch("{pointofinterestid}")]
        public async Task<ActionResult> PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId,
            JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId)) return NotFound();

            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if (pointOfInterestEntity == null) return NotFound();

            var pointOfInterestToPatch = _mapper.Map<PointOfInterestForUpdateDto>(pointOfInterestEntity);

            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!TryValidateModel(pointOfInterestToPatch)) return BadRequest();

            // We don't use _mapper.Map<>. Here it is a different way. In this case, all unassigned properties in our 
            // updated entity will be automatically assigned using pointOfInterestToPatch properties.
            // For example: if we don't have "Description" property in our pointOfInterestEntity, the "Description"
            // property will be taken from pointOfInterestToPatch.
            _mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);
            await _cityInfoRepository.SafeChangesAsync();

            return NoContent();
        }

        [HttpDelete("{pointofinterestid}")]
        public async Task<ActionResult> DeletePointOfInterest(int cityId, int pointOfInterestId)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId)) return NotFound();

            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if (pointOfInterestEntity == null) return NotFound();

            // We can define DeletePointOfInterest as a non-async method, because we don't have a I/O operation here.
            // If we made a call of an async method in DeletePointOfInterest method, then this method had to be async as well.
            // In that case it don't have to.
            _cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);
            await _cityInfoRepository.SafeChangesAsync();

            _mailService.Send("Point of interest deleted.", $"Point of interest {pointOfInterestEntity.Name} with id " +
                $"{pointOfInterestEntity.Id} was deleted.");

            return NoContent();
        }
    }
}
