using System;
using System.ComponentModel.DataAnnotations;

namespace GigHub.Dtos
{
    public class GigDto
    {
        public int Id { get; set; }
        public bool IsCanceled { get; set; }
        public UserDto Artist { get; set; }
        [Required]
        public DateTime DateTime { get; set; }
        public String Venue { get; set; }
        public GenreDto Genre { get; set; }

    }
}