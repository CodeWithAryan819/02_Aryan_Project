using _02_Aryan_Project.Data;
using _02_Aryan_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _02_Aryan_Project.Controllers
{
    [Authorize] // Ensures that only authenticated users can access this controller
    [Route("api/[controller]/{action}")] // Defines the route pattern for API endpoints
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Constructor to inject the database context
        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Retrieve all bookings from the database (Admin only)
        [HttpGet]
        public IActionResult GetBookings()
        {
            // Check if the logged-in user has an Admin role
            if (!User.IsInRole("Admin"))
            {
                return Unauthorized("Only admins can view all bookings.");
            }

            return Ok(_context.Bookings); // Return all bookings
        }

        // Retrieve a specific booking by its ID (Admin only)
        [HttpGet("{id}")]
        public IActionResult GetById(int? id)
        {
            // Check if the logged-in user has an Admin role
            if (!User.IsInRole("Admin"))
            {
                return Unauthorized("Only admins can view this booking.");
            }

            // Find the booking by ID
            var booking = _context.Bookings.FirstOrDefault(b => b.BookingID == id);
            if (booking == null)
                return Problem(detail: "Booking with Id " + id + " is not found.", statusCode: 404);

            return Ok(booking); // Return the found booking
        }

        // Create a new booking entry
        [HttpPost]
        public IActionResult Post(Booking booking)
        {
            _context.Bookings.Add(booking); // Add the booking to the database
            _context.SaveChanges(); // Save changes

            // Return the created booking with its generated ID
            return CreatedAtAction("GetById", new { id = booking.BookingID }, booking);
        }

        // Update an existing booking by its ID
        [HttpPut]
        public IActionResult Put(int? id, Booking booking)
        {
            // Find the booking in the database
            var entity = _context.Bookings.FirstOrDefault(b => b.BookingID == id);
            if (entity == null)
                return Problem(detail: "Booking with Id " + id + " is not found.", statusCode: 404);

            // Update the booking details
            entity.FacilityDescription = booking.FacilityDescription;
            entity.BookingDateFrom = booking.BookingDateFrom;
            entity.BookingDateTo = booking.BookingDateTo;
            entity.BookedBy = booking.BookedBy;
            entity.BookingStatus = booking.BookingStatus;

            _context.SaveChanges(); // Save changes

            return Ok(entity); // Return the updated booking
        }

        // Delete a booking by its ID
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            // Find the booking in the database
            var entity = _context.Bookings.FirstOrDefault(b => b.BookingID == id);
            if (entity == null)
                return Problem(detail: "Booking with Id " + id + " is not found.", statusCode: 404);

            _context.Bookings.Remove(entity); // Remove the booking from the database
            _context.SaveChanges(); // Save changes

            return Ok(entity); // Return the deleted booking
        }
    }
}
