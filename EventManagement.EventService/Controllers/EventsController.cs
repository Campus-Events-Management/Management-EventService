using AutoMapper;
using EventManagement.EventService.Models;
using EventManagement.EventService.Services;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace EventManagement.EventService.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<EventsController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public EventsController(
            IEventRepository eventRepository,
            IMapper mapper,
            ILogger<EventsController> logger,
            IWebHostEnvironment environment,
            IConfiguration configuration)
        {
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetEvents([FromQuery] bool? isPast)
        {
            try
            {
                IEnumerable<Event> events;

                if (isPast.HasValue)
                {
                    events = isPast.Value
                        ? await _eventRepository.GetPastEventsAsync()
                        : await _eventRepository.GetUpcomingEventsAsync();
                }
                else
                {
                    events = await _eventRepository.GetAllEventsAsync();
                }

                return Ok(_mapper.Map<IEnumerable<EventDto>>(events));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving events");
                return StatusCode(500, "An error occurred while retrieving events");
            }
        }

        [HttpGet("{id}", Name = "GetEventById")]
        public async Task<ActionResult<EventDto>> GetEventById(Guid id)
        {
            try
            {
                var eventEntity = await _eventRepository.GetEventByIdAsync(id);
                if (eventEntity == null)
                {
                    return NotFound();
                }

                return Ok(_mapper.Map<EventDto>(eventEntity));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving event with ID {id}");
                return StatusCode(500, "An error occurred while retrieving the event");
            }
        }

        [HttpPost]
        public async Task<ActionResult<EventDto>> CreateEvent([FromBody] CreateEventDto createEventDto)
        {
            try
            {
                var eventEntity = _mapper.Map<Event>(createEventDto);
                eventEntity = await _eventRepository.AddEventAsync(eventEntity);

                var eventToReturn = _mapper.Map<EventDto>(eventEntity);
                return CreatedAtRoute("GetEventById", new { id = eventToReturn.Id }, eventToReturn);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating event");
                return StatusCode(500, "An error occurred while creating the event");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<EventDto>> UpdateEvent(Guid id, [FromBody] UpdateEventDto updateEventDto)
        {
            try
            {
                // Check if event exists
                var eventEntity = await _eventRepository.GetEventByIdAsync(id);
                if (eventEntity == null)
                {
                    return NotFound();
                }

                // Map updateEventDto to eventEntity
                _mapper.Map(updateEventDto, eventEntity);

                // Update event
                eventEntity = await _eventRepository.UpdateEventAsync(eventEntity);
                if (eventEntity == null)
                {
                    return StatusCode(500, "An error occurred while updating the event");
                }

                return Ok(_mapper.Map<EventDto>(eventEntity));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating event with ID {id}");
                return StatusCode(500, "An error occurred while updating the event");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEvent(Guid id)
        {
            try
            {
                var result = await _eventRepository.DeleteEventAsync(id);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting event with ID {id}");
                return StatusCode(500, "An error occurred while deleting the event");
            }
        }

        [HttpPut("{id}/registration")]
        public async Task<ActionResult> UpdateRegistration(Guid id, [FromBody] UpdateRegistrationDto updateRegistrationDto)
        {
            try
            {
                var result = await _eventRepository.UpdateRegistrationCountAsync(id, updateRegistrationDto.IncrementBy);
                if (!result)
                {
                    // Check if event exists
                    var eventExists = await _eventRepository.EventExistsAsync(id);
                    if (!eventExists)
                    {
                        return NotFound();
                    }

                    // If event exists but update failed, it's likely due to capacity constraint
                    return BadRequest("Cannot update registration count. The event may be at capacity.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating registration for event with ID {id}");
                return StatusCode(500, "An error occurred while updating the registration");
            }
        }

        [HttpGet("{id}/has-capacity")]
        public async Task<ActionResult<bool>> HasCapacity(Guid id)
        {
            try
            {
                // Check if event exists
                var eventExists = await _eventRepository.EventExistsAsync(id);
                if (!eventExists)
                {
                    return NotFound();
                }

                var hasCapacity = await _eventRepository.HasAvailableCapacityAsync(id);
                return Ok(hasCapacity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking capacity for event with ID {id}");
                return StatusCode(500, "An error occurred while checking the event capacity");
            }
        }

        [HttpPut("{id}/bookings")]
        public async Task<ActionResult> UpdateBookingCount(Guid id, [FromBody] BookingCountUpdateDto request)
        {
            try
            {
                var eventEntity = await _eventRepository.GetEventByIdAsync(id);
                
                if (eventEntity == null)
                {
                    return NotFound();
                }
                
                if (request.IsIncrement)
                {
                    // Check capacity
                    if (eventEntity.Registered >= eventEntity.Capacity)
                    {
                        return BadRequest("Event is at full capacity");
                    }
                    
                    eventEntity.Registered++;
                }
                else
                {
                    // Ensure we don't go negative
                    if (eventEntity.Registered > 0)
                    {
                        eventEntity.Registered--;
                    }
                }
                
                var updatedEvent = await _eventRepository.UpdateEventAsync(eventEntity);
                
                if (updatedEvent != null)
                {
                    return Ok();
                }
                else
                {
                    return StatusCode(500, "Failed to update event booking count");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating booking count for event with ID {id}");
                return StatusCode(500, "An error occurred while updating the booking count");
            }
        }

        [HttpPost("{id}/image")]
        public async Task<ActionResult> UploadEventImage(Guid id, [FromForm] IFormFile image)
        {
            try
            {
                _logger.LogInformation($"Image upload request received for event ID: {id}");
                
                // Check if event exists
                var eventEntity = await _eventRepository.GetEventByIdAsync(id);
                if (eventEntity == null)
                {
                    _logger.LogWarning($"Event with ID {id} not found during image upload");
                    return NotFound();
                }

                // Validate file
                if (image == null || image.Length == 0)
                {
                    _logger.LogWarning("No image file provided or empty file");
                    return BadRequest("No image file provided");
                }

                _logger.LogInformation($"Received image: {image.FileName}, Size: {image.Length}, Content-Type: {image.ContentType}");

                // Check file type
                var validImageTypes = new[] { "image/jpeg", "image/png", "image/gif" };
                if (!validImageTypes.Contains(image.ContentType))
                {
                    _logger.LogWarning($"Invalid file type: {image.ContentType}");
                    return BadRequest("Invalid file type. Only JPEG, PNG, and GIF images are allowed.");
                }

                // Check file size (max 5MB)
                if (image.Length > 5 * 1024 * 1024)
                {
                    _logger.LogWarning($"File too large: {image.Length} bytes");
                    return BadRequest("Image file is too large. Maximum size is 5MB.");
                }

                // Get the root path - use WebRootPath if available, otherwise use ContentRootPath
                string rootPath = _environment.WebRootPath;
                if (string.IsNullOrEmpty(rootPath))
                {
                    rootPath = _environment.ContentRootPath;
                    _logger.LogWarning($"WebRootPath is null, using ContentRootPath instead: {rootPath}");
                }

                // Create directory if it doesn't exist
                var uploadsFolder = Path.Combine(rootPath, "event-images");
                if (!Directory.Exists(uploadsFolder))
                {
                    _logger.LogInformation($"Creating directory: {uploadsFolder}");
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Create unique filename
                var uniqueFileName = $"{id}-{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                _logger.LogInformation($"Saving file to: {filePath}");

                // Save the file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }

                // Get the base URL from configuration or use default
                var baseUrl = _configuration["EventService:BaseUrl"] ?? $"{Request.Scheme}://{Request.Host}";
                var imageUrl = $"{baseUrl}/event-images/{uniqueFileName}";

                _logger.LogInformation($"Image URL: {imageUrl}");

                // Update the event with the new image URL
                eventEntity.ImageUrl = imageUrl;
                var updatedEvent = await _eventRepository.UpdateEventAsync(eventEntity);

                if (updatedEvent == null)
                {
                    _logger.LogError("Failed to update event with new image URL");
                    return StatusCode(500, "Failed to update event with new image URL");
                }

                _logger.LogInformation($"Image upload successful for event ID: {id}");
                return Ok(new { imageUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error uploading image for event with ID {id}");
                return StatusCode(500, $"An error occurred while uploading the image: {ex.Message}");
            }
        }
    }
} 