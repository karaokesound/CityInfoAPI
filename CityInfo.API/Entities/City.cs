using CityInfo.API.Models;
using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Entities
{
    public class City
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string? Descirption { get; set; }
        public ICollection<PointOfInterestDto> PointsOfInterest { get; set; }

        public City(string name)
        {
            Name = name;
            PointsOfInterest = new List<PointOfInterestDto>();
        }
    }
}
