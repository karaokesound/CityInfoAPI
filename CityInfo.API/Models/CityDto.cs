using System.Reflection.Metadata.Ecma335;

namespace CityInfo.API.Models
{
    public class CityDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Descirption { get; set; }
        public ICollection<PointOfInterestDto> PointsOfInterest { get; set; }
        public int NumberOfPointsOfInterest
        {
            get => PointsOfInterest.Count;
        }

        public CityDto()
        {
            PointsOfInterest = new List<PointOfInterestDto>();
        }
    }
}
