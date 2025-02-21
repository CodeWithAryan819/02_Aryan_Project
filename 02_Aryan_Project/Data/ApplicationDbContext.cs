using _02_Aryan_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace _02_Aryan_Project.Data
{
    public class ApplicationDbContext : DbContext 
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {
        }

        public DbSet<Booking>? Bookings { get; set; }
    }
}
