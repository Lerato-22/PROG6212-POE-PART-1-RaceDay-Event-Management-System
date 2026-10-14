using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.DTOs;

namespace RaceDay.Controllers
{
    [ApiController]
    [Route("api/profile")]
    public class ProfileController : ControllerBase
    {
        private readonly RaceDayContext _context;

        public ProfileController(RaceDayContext context)
        {
            _context = context;
        }

        // GET /api/profile
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return Unauthorized(new { message = "Not logged in." });

            var user = await _context.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return NotFound(new { message = "User not found." });

            return Ok(new ProfileResponseDTO
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ProfilePictureUrl = user.ProfilePictureUrl,
                DateOfBirth = user.Profile?.DateOfBirth,
                Gender = user.Profile?.Gender,
                EmergencyContact = user.Profile?.EmergencyContact
            });
        }

        // PUT /api/profile
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDTO dto)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return Unauthorized(new { message = "Not logged in." });

            var user = await _context.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return NotFound(new { message = "User not found." });

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.PhoneNumber = dto.PhoneNumber;

            if (user.Profile == null)
                user.Profile = new Models.Profile { UserId = user.UserId };

            user.Profile.DateOfBirth = dto.DateOfBirth;
            user.Profile.Gender = dto.Gender;
            user.Profile.EmergencyContact = dto.EmergencyContact;

            await _context.SaveChangesAsync();

            return Ok(new ProfileResponseDTO
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ProfilePictureUrl = user.ProfilePictureUrl,
                DateOfBirth = user.Profile.DateOfBirth,
                Gender = user.Profile.Gender,
                EmergencyContact = user.Profile.EmergencyContact
            });
        }
    }
}
