using _02_Aryan_Project.Data;
using _02_Aryan_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _02_Aryan_Project.Controllers
{
    [Authorize] // Ensures that only authenticated users can access this controller
    [Route("api/[controller]/{action}")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Constructor to inject the database context
        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all bookings from the database
        [HttpGet]
        public IActionResult GetBookings()
        {
            return Ok(_context.Bookings);
        }

        // Get a specific booking by its ID
        [HttpGet("{id}")]
        public IActionResult GetById(int? id)
        {
            var booking = _context.Bookings.FirstOrDefault(b => b.BookingID == id);
            if (booking == null)
                return Problem(detail: "Booking with Id " + id + " is not found.", statusCode: 404);

            return Ok(booking);
        }

        // Create a new booking
        [HttpPost]
        public IActionResult Post(Booking booking)
        {
            _context.Bookings.Add(booking);
            _context.SaveChanges();

            return CreatedAtAction("GetById", new { id = booking.BookingID }, booking);
        }

        // Update an existing booking
        [HttpPut]
        public IActionResult Put(int? id, Booking booking)
        {
            var entity = _context.Bookings.FirstOrDefault(b => b.BookingID == id);
            if (entity == null)
                return Problem(detail: "Booking with Id " + id + " is not found.", statusCode: 404);

            // Update booking details
            entity.FacilityDescription = booking.FacilityDescription;
            entity.BookingDateFrom = booking.BookingDateFrom;
            entity.BookingDateTo = booking.BookingDateTo;
            entity.BookedBy = booking.BookedBy;
            entity.BookingStatus = booking.BookingStatus;

            _context.SaveChanges();

            return Ok(entity);
        }

        // Delete a booking by its ID
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var entity = _context.Bookings.FirstOrDefault(b => b.BookingID == id);
            if (entity == null)
                return Problem(detail: "Booking with Id " + id + " is not found.", statusCode: 404);

            _context.Bookings.Remove(entity);
            _context.SaveChanges();

            return Ok(entity);
        }
    }
}
