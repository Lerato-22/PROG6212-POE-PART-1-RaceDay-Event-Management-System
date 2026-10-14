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
    public class EnrolmentsControllerTests
    {
        private RaceDayContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<RaceDayContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new RaceDayContext(options);
        }

        private EnrolmentsController GetController(RaceDayContext context, int? userId = null, string role = "Participant")
        {
            var controller = new EnrolmentsController(context);
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

        private async Task<(User organiser, User participant, Event eventItem, Category category)> SeedData(RaceDayContext context)
        {
            var organiser = new User
            {
                FirstName = "Sarah",
                LastName = "Nkosi",
                Email = "sarah@test.co.za",
                PasswordHash = "hashedpassword",
                Role = "Organiser"
            };
            context.Users.Add(organiser);

            var participant = new User
            {
                FirstName = "Thabo",
                LastName = "Dlamini",
                Email = "thabo@test.co.za",
                PasswordHash = "hashedpassword",
                Role = "Participant"
            };
            context.Users.Add(participant);
            await context.SaveChangesAsync();

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

            var category = new Category
            {
                EventId = eventItem.EventId,
                CategoryName = "Senior"
            };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            return (organiser, participant, eventItem, category);
        }

        [Fact]
        public async Task CreateEnrolment_AsParticipant_ReturnsCreated()
        {
            var context = GetInMemoryContext();
            var (_, participant, eventItem, category) = await SeedData(context);
            var controller = GetController(context, participant.UserId, "Participant");

            var dto = new CreateEnrolmentDTO
            {
                EventId = eventItem.EventId,
                CategoryId = category.CategoryId
            };

            var result = await controller.CreateEnrolment(dto);

            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task CreateEnrolment_AsOrganiser_ReturnsUnauthorized()
        {
            var context = GetInMemoryContext();
            var (organiser, _, eventItem, category) = await SeedData(context);
            var controller = GetController(context, organiser.UserId, "Organiser");

            var dto = new CreateEnrolmentDTO
            {
                EventId = eventItem.EventId,
                CategoryId = category.CategoryId
            };

            var result = await controller.CreateEnrolment(dto);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task CreateEnrolment_DuplicateEnrolment_ReturnsBadRequest()
        {
            var context = GetInMemoryContext();
            var (_, participant, eventItem, category) = await SeedData(context);
            var controller = GetController(context, participant.UserId, "Participant");

            var dto = new CreateEnrolmentDTO
            {
                EventId = eventItem.EventId,
                CategoryId = category.CategoryId
            };

            await controller.CreateEnrolment(dto);
            var result = await controller.CreateEnrolment(dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetMyEnrolments_AsParticipant_ReturnsOk()
        {
            var context = GetInMemoryContext();
            var (_, participant, _, _) = await SeedData(context);
            var controller = GetController(context, participant.UserId, "Participant");

            var result = await controller.GetMyEnrolments();

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetMyEnrolments_AsOrganiser_ReturnsUnauthorized()
        {
            var context = GetInMemoryContext();
            var (organiser, _, _, _) = await SeedData(context);
            var controller = GetController(context, organiser.UserId, "Organiser");

            var result = await controller.GetMyEnrolments();

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task CreateEnrolment_NonExistentEvent_ReturnsNotFound()
        {
            var context = GetInMemoryContext();
            var (_, participant, _, _) = await SeedData(context);
            var controller = GetController(context, participant.UserId, "Participant");

            var dto = new CreateEnrolmentDTO
            {
                EventId = 9999,
                CategoryId = 1
            };

            var result = await controller.CreateEnrolment(dto);

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
