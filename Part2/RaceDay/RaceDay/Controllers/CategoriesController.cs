using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Models;

namespace RaceDay.Controllers
{
    [ApiController]
    [Route("api/events/{eventId}/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly RaceDayContext _context;

        public CategoriesController(RaceDayContext context)
        {
            _context = context;
        }

        // GET /api/events/{eventId}/categories
        [HttpGet]
        public async Task<IActionResult> GetCategories(int eventId)
        {
            var eventExists = await _context.Events.AnyAsync(e => e.EventId == eventId);
            if (!eventExists)
                return NotFound(new { message = "Event not found." });

            var categories = await _context.Categories
                .Where(c => c.EventId == eventId)
                .ToListAsync();

            var response = categories.Select(c => new CategoryResponseDTO
            {
                CategoryId = c.CategoryId,
                EventId = c.EventId,
                CategoryName = c.CategoryName,
                Description = c.Description
            });

            return Ok(response);
        }

        // POST /api/events/{eventId}/categories
        [HttpPost]
        public async Task<IActionResult> CreateCategory(int eventId, [FromBody] CreateCategoryDTO dto)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Organiser")
                return Unauthorized(new { message = "Organiser access required." });

            var existingEvent = await _context.Events.FindAsync(eventId);
            if (existingEvent == null)
                return NotFound(new { message = "Event not found." });

            if (existingEvent.OrganiserId != userId)
                return StatusCode(403, new { message = "You can only add categories to your own events." });

            var category = new Category
            {
                EventId = eventId,
                CategoryName = dto.CategoryName,
                Description = dto.Description
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategories), new { eventId }, new CategoryResponseDTO
            {
                CategoryId = category.CategoryId,
                EventId = category.EventId,
                CategoryName = category.CategoryName,
                Description = category.Description
            });
        }

        // PUT /api/events/{eventId}/categories/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int eventId, int id, [FromBody] UpdateCategoryDTO dto)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Organiser")
                return Unauthorized(new { message = "Organiser access required." });

            var existingEvent = await _context.Events.FindAsync(eventId);
            if (existingEvent == null)
                return NotFound(new { message = "Event not found." });

            if (existingEvent.OrganiserId != userId)
                return StatusCode(403, new { message = "You can only update categories for your own events." });

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == id && c.EventId == eventId);

            if (category == null)
                return NotFound(new { message = "Category not found." });

            category.CategoryName = dto.CategoryName;
            category.Description = dto.Description;

            await _context.SaveChangesAsync();

            return Ok(new CategoryResponseDTO
            {
                CategoryId = category.CategoryId,
                EventId = category.EventId,
                CategoryName = category.CategoryName,
                Description = category.Description
            });
        }

        // DELETE /api/events/{eventId}/categories/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int eventId, int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Organiser")
                return Unauthorized(new { message = "Organiser access required." });

            var existingEvent = await _context.Events.FindAsync(eventId);
            if (existingEvent == null)
                return NotFound(new { message = "Event not found." });

            if (existingEvent.OrganiserId != userId)
                return StatusCode(403, new { message = "You can only delete categories from your own events." });

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == id && c.EventId == eventId);

            if (category == null)
                return NotFound(new { message = "Category not found." });

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Category deleted successfully." });
        }
    }
}