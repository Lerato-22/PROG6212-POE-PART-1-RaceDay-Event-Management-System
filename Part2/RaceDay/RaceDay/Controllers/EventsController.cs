using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Models;

namespace RaceDay.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly RaceDayContext _context;

        public EventsController(RaceDayContext context)
        {
            _context = context;
        }

        /// <summary>Returns all events. Optionally filter by eventType (Run, Walk, Cycle).</summary>
        // GET /api/events
        [HttpGet]
        public async Task<IActionResult> GetAllEvents([FromQuery] string? eventType)
        {
            var query = _context.Events
                .Include(e => e.Organiser)
                .Include(e => e.Categories)
                .Include(e => e.Enrolments)
                .AsQueryable();

            if (!string.IsNullOrEmpty(eventType))
                query = query.Where(e => e.EventType == eventType);

            var events = await query.ToListAsync();

            var response = events.Select(e => new EventResponseDTO
            {
                EventId = e.EventId,
                OrganiserId = e.OrganiserId,
                OrganizerName = $"{e.Organiser!.FirstName} {e.Organiser.LastName}",
                EventName = e.EventName,
                Description = e.Description,
                EventDate = e.EventDate,
                Location = e.Location,
                Distance = e.Distance,
                EventType = e.EventType,
                BannerImageUrl = e.BannerImageUrl,
                CreatedAt = e.CreatedAt,
                TotalEnrolments = e.Enrolments.Count,
                Categories = e.Categories.Select(c => new CategoryResponseDTO
                {
                    CategoryId = c.CategoryId,
                    EventId = c.EventId,
                    CategoryName = c.CategoryName,
                    Description = c.Description
                }).ToList()
            });

            return Ok(response);
        }

        /// <summary>Returns all events created by the logged-in Organiser.</summary>
        // GET /api/events/my
        [HttpGet("my")]
        public async Task<IActionResult> GetMyEvents()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Organiser")
                return Unauthorized(new { message = "Organiser access required." });

            var events = await _context.Events
                .Include(e => e.Categories)
                .Include(e => e.Enrolments)
                .Where(e => e.OrganiserId == userId)
                .ToListAsync();

            var response = events.Select(e => new EventResponseDTO
            {
                EventId = e.EventId,
                OrganiserId = e.OrganiserId,
                EventName = e.EventName,
                Description = e.Description,
                EventDate = e.EventDate,
                Location = e.Location,
                Distance = e.Distance,
                EventType = e.EventType,
                BannerImageUrl = e.BannerImageUrl,
                CreatedAt = e.CreatedAt,
                TotalEnrolments = e.Enrolments.Count,
                Categories = e.Categories.Select(c => new CategoryResponseDTO
                {
                    CategoryId = c.CategoryId,
                    EventId = c.EventId,
                    CategoryName = c.CategoryName,
                    Description = c.Description
                }).ToList()
            });

            return Ok(response);
        }

        /// <summary>Returns a single event by ID including its categories and enrolment count.</summary>
        // GET /api/events/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvent(int id)
        {
            var e = await _context.Events
                .Include(e => e.Organiser)
                .Include(e => e.Categories)
                .Include(e => e.Enrolments)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (e == null)
                return NotFound(new { message = "Event not found." });

            return Ok(new EventResponseDTO
            {
                EventId = e.EventId,
                OrganiserId = e.OrganiserId,
                OrganizerName = $"{e.Organiser!.FirstName} {e.Organiser.LastName}",
                EventName = e.EventName,
                Description = e.Description,
                EventDate = e.EventDate,
                Location = e.Location,
                Distance = e.Distance,
                EventType = e.EventType,
                BannerImageUrl = e.BannerImageUrl,
                CreatedAt = e.CreatedAt,
                TotalEnrolments = e.Enrolments.Count,
                Categories = e.Categories.Select(c => new CategoryResponseDTO
                {
                    CategoryId = c.CategoryId,
                    EventId = c.EventId,
                    CategoryName = c.CategoryName,
                    Description = c.Description
                }).ToList()
            });
        }

        /// <summary>Creates a new event. Organiser role required.</summary>
        // POST /api/events
        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventDTO dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Request body cannot be empty." });

            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Organiser")
                return Unauthorized(new { message = "Organiser access required." });

            var newEvent = new Event
            {
                OrganiserId = userId.Value,
                EventName = dto.EventName,
                Description = dto.Description,
                EventDate = dto.EventDate,
                Location = dto.Location,
                Distance = dto.Distance,
                EventType = dto.EventType
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEvent), new { id = newEvent.EventId }, new EventResponseDTO
            {
                EventId = newEvent.EventId,
                OrganiserId = newEvent.OrganiserId,
                EventName = newEvent.EventName,
                Description = newEvent.Description,
                EventDate = newEvent.EventDate,
                Location = newEvent.Location,
                Distance = newEvent.Distance,
                EventType = newEvent.EventType,
                CreatedAt = newEvent.CreatedAt
            });
        }

        /// <summary>Updates an existing event. Organiser must own the event.</summary>
        // PUT /api/events/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] UpdateEventDTO dto)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Organiser")
                return Unauthorized(new { message = "Organiser access required." });

            var existingEvent = await _context.Events.FindAsync(id);
            if (existingEvent == null)
                return NotFound(new { message = "Event not found." });

            if (existingEvent.OrganiserId != userId)
                return StatusCode(403, new { message = "You can only update your own events." });

            if (!new[] { "Run", "Walk", "Cycle" }.Contains(dto.EventType))
                return BadRequest(new { message = "EventType must be Run, Walk, or Cycle." });

            existingEvent.EventName = dto.EventName;
            existingEvent.Description = dto.Description;
            existingEvent.EventDate = dto.EventDate;
            existingEvent.Location = dto.Location;
            existingEvent.Distance = dto.Distance;
            existingEvent.EventType = dto.EventType;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Event updated successfully.", eventId = id });
        }

        /// <summary>Deletes an event. Organiser must own the event.</summary>
        // DELETE /api/events/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Organiser")
                return Unauthorized(new { message = "Organiser access required." });

            var existingEvent = await _context.Events.FindAsync(id);
            if (existingEvent == null)
                return NotFound(new { message = "Event not found." });

            if (existingEvent.OrganiserId != userId)
                return StatusCode(403, new { message = "You can only delete your own events." });

            _context.Events.Remove(existingEvent);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Event deleted successfully." });
        }
    }
}
