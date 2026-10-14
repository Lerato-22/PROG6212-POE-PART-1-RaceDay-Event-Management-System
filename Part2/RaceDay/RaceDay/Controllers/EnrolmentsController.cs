using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Models;

namespace RaceDay.Controllers
{
    [ApiController]
    [Route("api/enrolments")]
    public class EnrolmentsController : ControllerBase
    {
        private readonly RaceDayContext _context;

        public EnrolmentsController(RaceDayContext context)
        {
            _context = context;
        }

        // POST /api/enrolments
        [HttpPost]
        public async Task<IActionResult> CreateEnrolment([FromBody] CreateEnrolmentDTO dto)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Participant")
                return Unauthorized(new { message = "Participant access required." });

            var eventExists = await _context.Events.AnyAsync(e => e.EventId == dto.EventId);
            if (!eventExists)
                return NotFound(new { message = "Event not found." });

            var categoryExists = await _context.Categories
                .AnyAsync(c => c.CategoryId == dto.CategoryId && c.EventId == dto.EventId);
            if (!categoryExists)
                return NotFound(new { message = "Category not found for this event." });

            var alreadyEnrolled = await _context.Enrolments
                .AnyAsync(e => e.ParticipantId == userId && e.EventId == dto.EventId);
            if (alreadyEnrolled)
                return BadRequest(new { message = "You are already enrolled in this event." });

            var enrolment = new Enrolment
            {
                ParticipantId = userId.Value,
                EventId = dto.EventId,
                CategoryId = dto.CategoryId,
                Status = "Pending"
            };

            _context.Enrolments.Add(enrolment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMyEnrolments), new EnrolmentResponseDTO
            {
                EnrolmentId = enrolment.EnrolmentId,
                ParticipantId = enrolment.ParticipantId,
                EventId = enrolment.EventId,
                CategoryId = enrolment.CategoryId,
                Status = enrolment.Status,
                EnrolledAt = enrolment.EnrolledAt
            });
        }

        // GET /api/enrolments/my
        [HttpGet("my")]
        public async Task<IActionResult> GetMyEnrolments()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Participant")
                return Unauthorized(new { message = "Participant access required." });

            var enrolments = await _context.Enrolments
                .Include(e => e.Event)
                .Include(e => e.Category)
                .Where(e => e.ParticipantId == userId)
                .ToListAsync();

            var response = enrolments.Select(e => new EnrolmentResponseDTO
            {
                EnrolmentId = e.EnrolmentId,
                ParticipantId = e.ParticipantId,
                EventId = e.EventId,
                EventName = e.Event!.EventName,
                EventDate = e.Event.EventDate,
                CategoryId = e.CategoryId,
                CategoryName = e.Category!.CategoryName,
                Status = e.Status,
                EnrolledAt = e.EnrolledAt
            });

            return Ok(response);
        }

        // GET /api/events/{eventId}/enrolments
        [HttpGet("/api/events/{eventId}/enrolments")]
        public async Task<IActionResult> GetEventEnrolments(int eventId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Organiser")
                return Unauthorized(new { message = "Organiser access required." });

            var existingEvent = await _context.Events.FindAsync(eventId);
            if (existingEvent == null)
                return NotFound(new { message = "Event not found." });

            if (existingEvent.OrganiserId != userId)
                return StatusCode(403, new { message = "You can only view enrolments for your own events." });

            var enrolments = await _context.Enrolments
                .Include(e => e.Participant)
                .Include(e => e.Category)
                .Where(e => e.EventId == eventId)
                .ToListAsync();

            var response = enrolments.Select(e => new EnrolmentResponseDTO
            {
                EnrolmentId = e.EnrolmentId,
                ParticipantId = e.ParticipantId,
                ParticipantName = $"{e.Participant!.FirstName} {e.Participant.LastName}",
                EventId = e.EventId,
                CategoryId = e.CategoryId,
                CategoryName = e.Category!.CategoryName,
                Status = e.Status,
                EnrolledAt = e.EnrolledAt
            });

            return Ok(response);
        }

        // PUT /api/enrolments/{id}/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateEnrolmentStatus(int id, [FromBody] UpdateEnrolmentStatusDTO dto)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Organiser")
                return Unauthorized(new { message = "Organiser access required." });

            if (!new[] { "Pending", "Confirmed", "Cancelled" }.Contains(dto.Status))
                return BadRequest(new { message = "Status must be Pending, Confirmed, or Cancelled." });

            var enrolment = await _context.Enrolments
                .Include(e => e.Event)
                .FirstOrDefaultAsync(e => e.EnrolmentId == id);

            if (enrolment == null)
                return NotFound(new { message = "Enrolment not found." });

            if (enrolment.Event!.OrganiserId != userId)
                return StatusCode(403, new { message = "You can only update enrolments for your own events." });

            enrolment.Status = dto.Status;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Enrolment status updated.", status = dto.Status });
        }

        // DELETE /api/enrolments/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnrolment(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Participant")
                return Unauthorized(new { message = "Participant access required." });

            var enrolment = await _context.Enrolments
                .FirstOrDefaultAsync(e => e.EnrolmentId == id && e.ParticipantId == userId);

            if (enrolment == null)
                return NotFound(new { message = "Enrolment not found." });

            _context.Enrolments.Remove(enrolment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Enrolment cancelled successfully." });
        }
    }
}
