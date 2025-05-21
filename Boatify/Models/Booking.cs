using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChuyenDI.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        public int UserId { get; set; }

        public int TicketId { get; set; }

        public int SeatId { get; set; }

        [Required]
        [StringLength(100)]
        public string PassengerName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string PassengerEmail { get; set; }

        [StringLength(15)]
        public string PassengerPhone { get; set; }

        public DateTime BookingTime { get; set; }

        [Required]
        [Column(TypeName = "decimal")]
        public decimal TotalPrice { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("TicketId")]
        public virtual Ticket Ticket { get; set; }

        [ForeignKey("SeatId")]
        public virtual Seat Seat { get; set; }
    }
}