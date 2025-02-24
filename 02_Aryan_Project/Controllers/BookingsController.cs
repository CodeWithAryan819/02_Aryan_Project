using _02_Aryan_Project.Data;
using _02_Aryan_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _02_Aryan_Project.Controllers
{
    [Authorize]
    [Route("api/[controller]/{action}")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context) 
        {
            _context = context;
        }

        [HttpGet]

        public IActionResult GetBookings()
        {
            return Ok(_context.Bookings);
        }

        [HttpGet("{id}")]

        public IActionResult GetById(int? id) 
        {
            var bookings = _context.Bookings.FirstOrDefault(b => b.BookingID == id);
            if (bookings == null)
                return Problem(detail: "Booking with Id " + id + " is not found.", statusCode: 404);

            return Ok(bookings);
        }

        [HttpPost]

        public IActionResult Post(Booking booking) 
        {
            _context.Bookings.Add(booking);
            _context.SaveChanges();

            return CreatedAtAction("GetById", new {id = booking.BookingID}, booking);
        }

        [HttpPut]

        public IActionResult Put(int? id, Booking booking) 
        {
            var entity = _context.Bookings.FirstOrDefault(b => b.BookingID == id);
            if (entity == null)
                return Problem(detail: "Booking with Id " + id + " is not found.", statusCode: 404);

            entity.FacilityDescription = booking.FacilityDescription;
            entity.BookingDateFrom = booking.BookingDateFrom;
            entity.BookingDateTo = booking.BookingDateTo;
            entity.BookedBy = booking.BookedBy;
            entity.BookingStatus = booking.BookingStatus;

            _context.SaveChanges();

            return Ok(entity);
        }

        [HttpDelete]

        public IActionResult Delete(int? id, Booking booking) 
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
