using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Models;

namespace RaceDay.Controllers
{
    [ApiController]
    [Route("api/results")]
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayContext _context;

        public ResultsController(RaceDayContext context)
        {
            _context = context;
        }

        /// <summary>Captures a finish time and position for an enrolled Participant. Organiser role required.</summary>
        // POST /api/results
        [HttpPost]
        public async Task<IActionResult> CreateResult([FromBody] CreateResultDTO dto)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Organiser")
                return Unauthorized(new { message = "Organiser access required." });

            var enrolment = await _context.Enrolments
                .Include(e => e.Event)
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.EnrolmentId == dto.EnrolmentId);

            if (enrolment == null)
                return NotFound(new { message = "Enrolment not found." });

            if (enrolment.Event!.OrganiserId != userId)
                return StatusCode(403, new { message = "You can only capture results for your own events." });

            var resultExists = await _context.Results
                .AnyAsync(r => r.EnrolmentId == dto.EnrolmentId);
            if (resultExists)
                return BadRequest(new { message = "Result already exists for this enrolment." });

            var result = new Result
            {
                EnrolmentId = dto.EnrolmentId,
                FinishTime = dto.FinishTime,
                FinishingPosition = dto.FinishingPosition,
                TotalFinishers = dto.TotalFinishers
            };

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMyResults), new ResultResponseDTO
            {
                ResultId = result.ResultId,
                EnrolmentId = result.EnrolmentId,
                FinishTime = result.FinishTime,
                FinishingPosition = result.FinishingPosition,
                TotalFinishers = result.TotalFinishers,
                RecordedAt = result.RecordedAt
            });
        }

        /// <summary>Updates an existing result. Organiser must own the event.</summary>
        // PUT /api/results/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateResult(int id, [FromBody] UpdateResultDTO dto)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Organiser")
                return Unauthorized(new { message = "Organiser access required." });

            var result = await _context.Results
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e!.Event)
                .FirstOrDefaultAsync(r => r.ResultId == id);

            if (result == null)
                return NotFound(new { message = "Result not found." });

            if (result.Enrolment!.Event!.OrganiserId != userId)
                return StatusCode(403, new { message = "You can only update results for your own events." });

            result.FinishTime = dto.FinishTime;
            result.FinishingPosition = dto.FinishingPosition;
            result.TotalFinishers = dto.TotalFinishers;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Result updated successfully.", resultId = id });
        }

        /// <summary>Returns all results for the logged-in Participant across all completed events.</summary>
        // GET /api/results/my
        [HttpGet("my")]
        public async Task<IActionResult> GetMyResults()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Participant")
                return Unauthorized(new { message = "Participant access required." });

            var results = await _context.Results
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e!.Event)
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e!.Category)
                .Where(r => r.Enrolment!.ParticipantId == userId)
                .ToListAsync();

            var response = results.Select(r => new ResultResponseDTO
            {
                ResultId = r.ResultId,
                EnrolmentId = r.EnrolmentId,
                EventName = r.Enrolment!.Event!.EventName,
                EventDate = r.Enrolment.Event.EventDate,
                CategoryName = r.Enrolment!.Category!.CategoryName,
                FinishTime = r.FinishTime,
                FinishingPosition = r.FinishingPosition,
                TotalFinishers = r.TotalFinishers,
                RecordedAt = r.RecordedAt
            });

            return Ok(response);
        }

        /// <summary>Returns all results for a specific event. Organiser must own the event.</summary>
        // GET /api/events/{eventId}/results
        [HttpGet("/api/events/{eventId}/results")]
        public async Task<IActionResult> GetEventResults(int eventId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Organiser")
                return Unauthorized(new { message = "Organiser access required." });

            var existingEvent = await _context.Events.FindAsync(eventId);
            if (existingEvent == null)
                return NotFound(new { message = "Event not found." });

            if (existingEvent.OrganiserId != userId)
                return StatusCode(403, new { message = "You can only view results for your own events." });

            var results = await _context.Results
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e!.Participant)
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e!.Category)
                .Where(r => r.Enrolment!.EventId == eventId)
                .ToListAsync();

            var response = results.Select(r => new ResultResponseDTO
            {
                ResultId = r.ResultId,
                EnrolmentId = r.EnrolmentId,
                ParticipantName = $"{r.Enrolment!.Participant!.FirstName} {r.Enrolment.Participant.LastName}",
                EventName = existingEvent.EventName,
                EventDate = existingEvent.EventDate,
                CategoryName = r.Enrolment!.Category!.CategoryName,
                FinishTime = r.FinishTime,
                FinishingPosition = r.FinishingPosition,
                TotalFinishers = r.TotalFinishers,
                RecordedAt = r.RecordedAt
            });

            return Ok(response);
        }
    }
}