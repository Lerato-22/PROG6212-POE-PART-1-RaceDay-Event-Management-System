using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Controllers;
using RaceDay.Data;
using RaceDay.DTOs;
using Xunit;

namespace RaceDay.Tests
{
    public class AuthControllerTests
    {
        private RaceDayContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<RaceDayContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new RaceDayContext(options);
        }

        private AuthController GetController(RaceDayContext context)
        {
            var controller = new AuthController(context);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            controller.HttpContext.Session = new TestSession();
            return controller;
        }

        [Fact]
        public async Task Register_ValidOrganiser_ReturnsCreated()
        {
            var context = GetInMemoryContext();
            var controller = GetController(context);

            var dto = new RegisterDTO
            {
                FirstName = "Sarah",
                LastName = "Nkosi",
                Email = "sarah@test.co.za",
                Password = "Password123",
                Role = "Organiser",
                PhoneNumber = "0821234567"
            };

            var result = await controller.Register(dto);

            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task Register_InvalidRole_ReturnsBadRequest()
        {
            var context = GetInMemoryContext();
            var controller = GetController(context);

            var dto = new RegisterDTO
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@test.co.za",
                Password = "Password123",
                Role = "Admin"
            };

            var result = await controller.Register(dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Register_DuplicateEmail_ReturnsConflict()
        {
            var context = GetInMemoryContext();
            var controller = GetController(context);

            var dto = new RegisterDTO
            {
                FirstName = "Sarah",
                LastName = "Nkosi",
                Email = "sarah@test.co.za",
                Password = "Password123",
                Role = "Organiser"
            };

            await controller.Register(dto);
            var result = await controller.Register(dto);

            Assert.IsType<ConflictObjectResult>(result);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOk()
        {
            var context = GetInMemoryContext();
            var controller = GetController(context);

            var registerDto = new RegisterDTO
            {
                FirstName = "Sarah",
                LastName = "Nkosi",
                Email = "sarah@test.co.za",
                Password = "Password123",
                Role = "Organiser"
            };
            await controller.Register(registerDto);

            var loginDto = new LoginDTO
            {
                Email = "sarah@test.co.za",
                Password = "Password123"
            };

            var result = await controller.Login(loginDto);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Login_InvalidPassword_ReturnsUnauthorized()
        {
            var context = GetInMemoryContext();
            var controller = GetController(context);

            var registerDto = new RegisterDTO
            {
                FirstName = "Sarah",
                LastName = "Nkosi",
                Email = "sarah@test.co.za",
                Password = "Password123",
                Role = "Organiser"
            };
            await controller.Register(registerDto);

            var loginDto = new LoginDTO
            {
                Email = "sarah@test.co.za",
                Password = "WrongPassword"
            };

            var result = await controller.Login(loginDto);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }
    }
}
