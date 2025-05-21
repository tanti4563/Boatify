using System;
using System.ComponentModel.DataAnnotations;

namespace ChuyenDI.Models.ViewModels
{
    public class SearchViewModel
    {
        [Required]
        [Display(Name = "From")]
        public string Origin { get; set; }

        [Required]
        [Display(Name = "To")]
        public string Destination { get; set; }

        [Required]
        [Display(Name = "Departure Date")]
        [DataType(DataType.Date)]
        public DateTime DepartureDate { get; set; }

        [Display(Name = "Number of Passengers")]
        public int Passengers { get; set; } = 1;
    }
}