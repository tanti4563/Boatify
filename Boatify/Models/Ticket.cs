using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChuyenDI.Models
{
    public class Ticket
    {
        public int TicketId { get; set; }

        public int RouteId { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }

        [Required]
        public DateTime ArrivalTime { get; set; }

        [Required]
        [Column(TypeName = "decimal")]
        public decimal BasePrice { get; set; }

        public int TotalSeats { get; set; }

        [ForeignKey("RouteId")]
        public virtual Route Route { get; set; }
    }
}