namespace CityInfo.API.Models
{
    public class CityDto
    {
        // We do not use Data Annotations here, because CityDto is only to GET data, not to store it.
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
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
