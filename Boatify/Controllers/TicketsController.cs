using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ChuyenDI.Models;
using ChuyenDI.Models.ViewModels;

namespace ChuyenDI.Controllers
{
    public class TicketsController : Controller
    {
        private ChuyenDIDbContext db = new ChuyenDIDbContext();

        // GET: Tickets/Search
        public ActionResult Search()
        {
            var origins = db.Routes.Select(r => r.Origin).Distinct().ToList();
            var destinations = db.Routes.Select(r => r.Destination).Distinct().ToList();

            ViewBag.Origins = new SelectList(origins);
            ViewBag.Destinations = new SelectList(destinations);

            return View(new SearchViewModel { DepartureDate = DateTime.Today });
        }

        // POST: Tickets/Search
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Search(SearchViewModel model)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Results", new
                {
                    origin = model.Origin,
                    destination = model.Destination,
                    date = model.DepartureDate.ToString("yyyy-MM-dd"),
                    passengers = model.Passengers
                });
            }

            var origins = db.Routes.Select(r => r.Origin).Distinct().ToList();
            var destinations = db.Routes.Select(r => r.Destination).Distinct().ToList();

            ViewBag.Origins = new SelectList(origins);
            ViewBag.Destinations = new SelectList(destinations);

            return View(model);
        }

        // GET: Tickets/Results
        public ActionResult Results(string origin, string destination, string date, int passengers = 1)
        {
            DateTime searchDate = DateTime.Parse(date);

            var tickets = db.Tickets
                .Include(t => t.Route)
                .Where(t => t.Route.Origin == origin &&
                       t.Route.Destination == destination &&
                       DbFunctions.TruncateTime(t.DepartureTime) == DbFunctions.TruncateTime(searchDate))
                .ToList();

            ViewBag.Origin = origin;
            ViewBag.Destination = destination;
            ViewBag.SearchDate = searchDate;
            ViewBag.Passengers = passengers;

            return View(tickets);
        }

        // GET: Tickets/SelectSeat/5
        public ActionResult SelectSeat(int id, int passengers = 1)
        {
            var ticket = db.Tickets
                .Include(t => t.Route)
                .FirstOrDefault(t => t.TicketId == id);

            if (ticket == null)
            {
                return HttpNotFound();
            }

            // Get ALL seats, not just available ones
            var seats = db.Seats
                .Where(s => s.TicketId == id)
                .ToList();

            ViewBag.Ticket = ticket;
            ViewBag.Passengers = passengers;

            return View(seats);
        }

        // GET: Tickets/Book/5
        //[Authorize]
        public ActionResult Book(int ticketId, int seatId)
        {
            var ticket = db.Tickets
                .Include(t => t.Route)
                .FirstOrDefault(t => t.TicketId == ticketId);

            var seat = db.Seats.Find(seatId);

            if (ticket == null || seat == null || !seat.IsAvailable)
            {
                return HttpNotFound();
            }

            string username = User.Identity.Name;
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = db.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var viewModel = new BookingViewModel
            {
                TicketId = ticketId,
                SeatId = seatId,
                PassengerName = $"{user.FirstName} {user.LastName}",
                PassengerEmail = user.Email,
                PassengerPhone = user.PhoneNumber,
                TotalPrice = ticket.BasePrice
            };

            ViewBag.Ticket = ticket;
            ViewBag.Seat = seat;

            return View(viewModel);
        }

        // POST: Tickets/Book
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize]
        public ActionResult Book(BookingViewModel model)
        {
            if (ModelState.IsValid)
            {
                string username = User.Identity.Name;
                if (string.IsNullOrEmpty(username))
                {
                    return RedirectToAction("Login", "Account");
                }

                var user = db.Users.FirstOrDefault(u => u.Username == username);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var seat = db.Seats.Find(model.SeatId);
                if (seat == null || !seat.IsAvailable)
                {
                    return HttpNotFound();
                }

                var booking = new Booking
                {
                    UserId = user.UserId,
                    TicketId = model.TicketId,
                    SeatId = model.SeatId,
                    PassengerName = model.PassengerName,
                    PassengerEmail = model.PassengerEmail,
                    PassengerPhone = model.PassengerPhone,
                    BookingTime = DateTime.Now,
                    TotalPrice = model.TotalPrice
                };

                // Mark seat as unavailable
                seat.IsAvailable = false;

                db.Bookings.Add(booking);
                db.SaveChanges();

                return RedirectToAction("BookingConfirmation", new { id = booking.BookingId });
            }

            var ticket = db.Tickets
                .Include(t => t.Route)
                .FirstOrDefault(t => t.TicketId == model.TicketId);

            var seatInfo = db.Seats.Find(model.SeatId);

            ViewBag.Ticket = ticket;
            ViewBag.Seat = seatInfo;

            return View(model);
        }
        // GET: Tickets/BookingConfirmation/5
        [Authorize]
        public ActionResult BookingConfirmation(int id)
        {
            var booking = db.Bookings
                .Include(b => b.User)
                .Include(b => b.Ticket)
                .Include(b => b.Ticket.Route)
                .Include(b => b.Seat)
                .FirstOrDefault(b => b.BookingId == id);

            if (booking == null)
            {
                return HttpNotFound();
            }

            return View(booking);
        }

        // GET: Tickets/History
        [Authorize]
        public ActionResult History()
        {
            string username = User.Identity.Name;
            var user = db.Users.FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var bookings = db.Bookings
                .Include(b => b.Ticket)
                .Include(b => b.Ticket.Route)
                .Include(b => b.Seat)
                .Where(b => b.UserId == user.UserId)
                .OrderByDescending(b => b.BookingTime)
                .ToList();

            return View(bookings);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}