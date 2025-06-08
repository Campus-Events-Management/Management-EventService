using System;
using System.ComponentModel.DataAnnotations;

namespace EventManagement.EventService.Models
{
    // Read model - used for GET operations
    public class EventDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public string? ImageUrl { get; set; }
        public int Capacity { get; set; }
        public int Registered { get; set; }
        public bool IsPast { get; set; }
    }

    // Create model - used for POST operations
    public class CreateEventDto
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(100)]
        public string Location { get; set; }

        public string? ImageUrl { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be at least 1")]
        public int Capacity { get; set; }
    }

    // Update model - used for PUT operations
    public class UpdateEventDto
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(100)]
        public string Location { get; set; }

        public string? ImageUrl { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be at least 1")]
        public int Capacity { get; set; }
    }

    // Registration update model - used by Booking service
    public class UpdateRegistrationDto
    {
        [Required]
        [Range(-100, 100, ErrorMessage = "Value must be between -100 and 100")]
        public int IncrementBy { get; set; }
    }
    
    // Booking count update model - used for the bookings endpoint
    public class BookingCountUpdateDto
    {
        [Required]
        public bool IsIncrement { get; set; }
    }
} 