namespace CityInfo.API.Models
{
    public class CityWithoutPointOfInterestDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Descirption { get; set; }
    }
}
