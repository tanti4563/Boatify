using System.ComponentModel.DataAnnotations;

namespace ChuyenDI.Models.ViewModels
{
    public class BookingViewModel
    {
        public int TicketId { get; set; }

        public int SeatId { get; set; }

        [Required]
        [Display(Name = "Passenger Name")]
        public string PassengerName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Passenger Email")]
        public string PassengerEmail { get; set; }

        [Display(Name = "Passenger Phone")]
        public string PassengerPhone { get; set; }

        public decimal TotalPrice { get; set; }
    }
}