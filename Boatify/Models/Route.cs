using System;
using System.ComponentModel.DataAnnotations;

namespace ChuyenDI.Models
{
    public class Route
    {
        public int RouteId { get; set; }

        [Required]
        [StringLength(100)]
        public string Origin { get; set; }

        [Required]
        [StringLength(100)]
        public string Destination { get; set; }

        public decimal? Distance { get; set; }
    }
}