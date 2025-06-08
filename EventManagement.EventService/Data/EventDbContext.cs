using EventManagement.EventService.Models;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.EventService.Data
{
    public class EventDbContext : DbContext
    {
        public EventDbContext(DbContextOptions<EventDbContext> options) : base(options)
        {
        }

        public DbSet<Event> Events { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed initial data
            modelBuilder.Entity<Event>().HasData(
                new Event
                {
                    Id = Guid.Parse("b0788d2f-8003-43c1-92a4-edc76a7c5dde"),
                    Title = "Technology Conference 2025",
                    Description = "Annual technology conference covering AI, ML, and cloud technologies.",
                    Date = DateTime.UtcNow.AddMonths(1),
                    Location = "Main Campus Auditorium",
                    ImageUrl = "https://example.com/images/tech-conf.jpg",
                    Capacity = 200,
                    Registered = 0
                },
                new Event
                {
                    Id = Guid.Parse("6313179f-7837-473a-a4d5-a5571b43e6a6"),
                    Title = "Career Fair",
                    Description = "Connect with over 50 employers looking to hire students and graduates.",
                    Date = DateTime.UtcNow.AddMonths(2),
                    Location = "Student Union Building",
                    ImageUrl = "https://example.com/images/career-fair.jpg",
                    Capacity = 500,
                    Registered = 0
                },
                new Event
                {
                    Id = Guid.Parse("bf3f3002-7e53-441e-8b76-f6280be284aa"),
                    Title = "Alumni Networking Night",
                    Description = "Connect with successful alumni and build your professional network.",
                    Date = DateTime.UtcNow.AddDays(-30),
                    Location = "Business School, Room 305",
                    ImageUrl = "https://example.com/images/alumni-event.jpg",
                    Capacity = 100,
                    Registered = 89
                }
            );
        }
    }
} 