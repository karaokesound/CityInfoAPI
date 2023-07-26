using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityInfo.API.Entities
{
    public class PointOfInterest
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        // Additionally we can use Data Annotations to explicitly define ForeignKey value.
        // [ForeignKey("CityId")]
        public City? City { get; set; }
        
        // This foreign key is included by default by EF Core. We don't have to write this line of code. It's optionally.
        public int CityId { get; set; }

        public PointOfInterest(string name)
        {
            Name = name;
        }
    }
}
