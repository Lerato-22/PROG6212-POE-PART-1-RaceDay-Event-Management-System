using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Controllers;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Models;
using Xunit;

namespace RaceDay.Tests
{
    public class EventsControllerTests
    {
        private RaceDayContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<RaceDayContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new RaceDayContext(options);
        }

        private EventsController GetController(RaceDayContext context, int? userId = null, string role = "Organiser")
        {
            var controller = new EventsController(context);
            var session = new TestSession();

            if (userId.HasValue)
            {
                session.SetInt32("UserId", userId.Value);
                session.SetString("Role", role);
            }

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            controller.HttpContext.Session = session;
            return controller;
        }

        private async Task<User> SeedOrganiser(RaceDayContext context)
        {
            var user = new User
            {
                FirstName = "Sarah",
                LastName = "Nkosi",
                Email = "sarah@test.co.za",
                PasswordHash = "hashedpassword",
                Role = "Organiser"
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();
            return user;
        }

        [Fact]
        public async Task GetAllEvents_ReturnsOk()
        {
            var context = GetInMemoryContext();
            var controller = GetController(context);

            var result = await controller.GetAllEvents(null);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task CreateEvent_AsOrganiser_ReturnsCreated()
        {
            var context = GetInMemoryContext();
            var organiser = await SeedOrganiser(context);
            var controller = GetController(context, organiser.UserId, "Organiser");

            var dto = new CreateEventDTO
            {
                EventName = "Soweto 10K",
                Description = "A fun run",
                EventDate = DateTime.Now.AddMonths(1),
                Location = "Soweto",
                Distance = 10,
                EventType = "Run"
            };

            var result = await controller.CreateEvent(dto);

            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task CreateEvent_AsParticipant_ReturnsUnauthorized()
        {
            var context = GetInMemoryContext();
            var controller = GetController(context, 1, "Participant");

            var dto = new CreateEventDTO
            {
                EventName = "Soweto 10K",
                Description = "A fun run",
                EventDate = DateTime.Now.AddMonths(1),
                Location = "Soweto",
                Distance = 10,
                EventType = "Run"
            };

            var result = await controller.CreateEvent(dto);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task CreateEvent_NotLoggedIn_ReturnsUnauthorized()
        {
            var context = GetInMemoryContext();
            var controller = GetController(context);

            var dto = new CreateEventDTO
            {
                EventName = "Soweto 10K",
                Description = "A fun run",
                EventDate = DateTime.Now.AddMonths(1),
                Location = "Soweto",
                Distance = 10,
                EventType = "Run"
            };

            var result = await controller.CreateEvent(dto);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task DeleteEvent_NotOwner_ReturnsForbidden()
        {
            var context = GetInMemoryContext();
            var organiser = await SeedOrganiser(context);

            var eventItem = new Event
            {
                OrganiserId = organiser.UserId,
                EventName = "Soweto 10K",
                EventDate = DateTime.Now.AddMonths(1),
                Location = "Soweto",
                Distance = 10,
                EventType = "Run"
            };
            context.Events.Add(eventItem);
            await context.SaveChangesAsync();

            // Different organiser trying to delete
            var controller = GetController(context, 999, "Organiser");
            var result = await controller.DeleteEvent(eventItem.EventId);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetEvent_NonExistentId_ReturnsNotFound()
        {
            var context = GetInMemoryContext();
            var controller = GetController(context);

            var result = await controller.GetEvent(9999);

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
