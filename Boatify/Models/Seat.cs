using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChuyenDI.Models
{
    public class Seat
    {
        public int SeatId { get; set; }

        public int TicketId { get; set; }

        [Required]
        [StringLength(10)]
        public string SeatNumber { get; set; }

        [StringLength(20)]
        public string SeatClass { get; set; }

        public bool IsAvailable { get; set; }

        [ForeignKey("TicketId")]
        public virtual Ticket Ticket { get; set; }
    }
}