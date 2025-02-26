using _02_Aryan_Project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace _02_Aryan_Project.Data
{
    /// <summary>
    /// ApplicationDbContext class, which extends IdentityDbContext to support ASP.NET Identity functionalities.
    /// This class is responsible for managing the database context, including user authentication and application data.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        /// <summary>
        /// Constructor for ApplicationDbContext.
        /// It receives database options and passes them to the base IdentityDbContext.
        /// </summary>
        /// <param name="options">Database context options</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// DbSet representing the Bookings table in the database.
        /// This property allows querying and saving instances of the Booking entity.
        /// </summary>
        public DbSet<Booking>? Bookings { get; set; }

        /// <summary>
        /// Configures the entity models and relationships when the database schema is being created.
        /// </summary>
        /// <param name="builder">ModelBuilder instance used to configure entities</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Call the base OnModelCreating method to ensure Identity tables are properly configured
            base.OnModelCreating(builder);
        }
    }
}
