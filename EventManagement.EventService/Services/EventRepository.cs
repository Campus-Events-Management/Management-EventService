using EventManagement.EventService.Data;
using EventManagement.EventService.Models;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.EventService.Services
{
    public class EventRepository : IEventRepository
    {
        private readonly EventDbContext _context;

        public EventRepository(EventDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Event>> GetAllEventsAsync()
        {
            return await _context.Events.ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetUpcomingEventsAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.Events
                .Where(e => e.Date > now)
                .OrderBy(e => e.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetPastEventsAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.Events
                .Where(e => e.Date <= now)
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }

        public async Task<Event> GetEventByIdAsync(Guid id)
        {
            return await _context.Events.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Event> AddEventAsync(Event eventEntity)
        {
            if (eventEntity == null)
            {
                throw new ArgumentNullException(nameof(eventEntity));
            }

            await _context.Events.AddAsync(eventEntity);
            await _context.SaveChangesAsync();
            return eventEntity;
        }

        public async Task<Event> UpdateEventAsync(Event eventEntity)
        {
            if (eventEntity == null)
            {
                throw new ArgumentNullException(nameof(eventEntity));
            }

            var existingEvent = await _context.Events.FirstOrDefaultAsync(e => e.Id == eventEntity.Id);
            if (existingEvent == null)
            {
                return null;
            }

            _context.Entry(existingEvent).CurrentValues.SetValues(eventEntity);
            await _context.SaveChangesAsync();
            return existingEvent;
        }

        public async Task<bool> DeleteEventAsync(Guid id)
        {
            var eventToDelete = await _context.Events.FirstOrDefaultAsync(e => e.Id == id);
            if (eventToDelete == null)
            {
                return false;
            }

            _context.Events.Remove(eventToDelete);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateRegistrationCountAsync(Guid id, int incrementBy)
        {
            var eventToUpdate = await _context.Events.FirstOrDefaultAsync(e => e.Id == id);
            if (eventToUpdate == null)
            {
                return false;
            }

            // Check if this would exceed capacity
            if (eventToUpdate.Registered + incrementBy > eventToUpdate.Capacity)
            {
                return false;
            }

            eventToUpdate.Registered += incrementBy;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> EventExistsAsync(Guid id)
        {
            return await _context.Events.AnyAsync(e => e.Id == id);
        }

        public async Task<bool> HasAvailableCapacityAsync(Guid id)
        {
            var eventEntity = await _context.Events.FirstOrDefaultAsync(e => e.Id == id);
            if (eventEntity == null)
            {
                return false;
            }

            return eventEntity.Registered < eventEntity.Capacity;
        }
    }
} 