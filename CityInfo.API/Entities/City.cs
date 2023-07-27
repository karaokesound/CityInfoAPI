﻿using CityInfo.API.Models;
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
        public string? Description { get; set; }
        public ICollection<PointOfInterest> PointsOfInterest { get; set; }

        public City(string name)
        {
            Name = name;
            PointsOfInterest = new List<PointOfInterest>();
        }
    }
}
