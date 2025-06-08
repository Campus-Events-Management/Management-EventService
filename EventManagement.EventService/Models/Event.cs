using System;
using System.ComponentModel.DataAnnotations;

namespace EventManagement.EventService.Models
{
    public class Event
    {
        [Key]
        public Guid Id { get; set; }

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
        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }

        [Required]
        public int Registered { get; set; } = 0;

        public bool IsPast => DateTime.UtcNow > Date;
    }
} 