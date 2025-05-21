using System.Data.Entity;

namespace ChuyenDI.Models
{
    public class ChuyenDIDbContext : DbContext
    {
        public ChuyenDIDbContext() : base("name=BoatifyDbContext")
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Booking> Bookings { get; set; }
    }
}