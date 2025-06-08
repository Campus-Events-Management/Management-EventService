using EventManagement.EventService.Models;

namespace EventManagement.EventService.Services
{
    public interface IEventRepository
    {
        Task<IEnumerable<Event>> GetAllEventsAsync();
        Task<IEnumerable<Event>> GetUpcomingEventsAsync();
        Task<IEnumerable<Event>> GetPastEventsAsync();
        Task<Event> GetEventByIdAsync(Guid id);
        Task<Event> AddEventAsync(Event eventEntity);
        Task<Event> UpdateEventAsync(Event eventEntity);
        Task<bool> DeleteEventAsync(Guid id);
        Task<bool> UpdateRegistrationCountAsync(Guid id, int incrementBy);
        Task<bool> EventExistsAsync(Guid id);
        Task<bool> HasAvailableCapacityAsync(Guid id);
    }
} 