using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using webApp.API.Controllers;
using webApp.Test.Utilities;
using FluentAssertions;
using webApp.Models;
using webApp.Data;
using System;
using Xunit;

namespace webApp.Test.API.Controllers
{
    public class UserControllerTest : DatabaseTest
    {
        [Fact]
        public async Task GetUser_ReturnsTheRequestedUser()
        {
            User testUser = new User() { UserId = "userid1", Username = "User1", DateOfBirth = DateTime.Today.Date };
            UsersController controller = new UsersController(NewContext);
            var controllerBase = (ControllerBase)controller;
            ActingAs(testUser, ref controllerBase);

            await using (MsSqlContext context = NewContext)
            {
                await context.Users.AddAsync(testUser);

                context.SaveChanges();
            }

            await using (MsSqlContext context = NewContext)
            {

                var response = await controller.GetUser("userid1");

                response.Value.Should().BeOfType<User>();

                response.Value.Should().BeEquivalentTo(testUser);
            }
        }

        [Fact]
        public async Task GetUser_ReturnsNotFoundWhenUserDoesNotExist()
        {
            User testUser = new User() { UserId = "userid", Username = "User1", DateOfBirth = DateTime.Today.Date };
            UsersController controller = new UsersController(NewContext);
            var controllerBase = (ControllerBase)controller;
            ActingAs(testUser, ref controllerBase);
            await using MsSqlContext context = NewContext;
            var response = await controller.GetUser("userid");

            response.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task PutUser_CanUpdateUsers()
        {
            User testUser = new User() { UserId = "userid", Username = "User1", DateOfBirth = DateTime.Today.Date };
            UsersController controller = new UsersController(NewContext);
            var controllerBase = (ControllerBase)controller;
            ActingAs(testUser, ref controllerBase);
            await using (MsSqlContext context = NewContext)
            {
                await context.Users.AddAsync(testUser);

                context.SaveChanges();
            }

            await using (MsSqlContext context = NewContext)
            {
                var response = await controller.PutUser(new User() { UserId = "userid", Username = "User", DateOfBirth = new DateTime(2000, 01, 02), Bio ="new bio"});

                response.Should().BeOfType<NoContentResult>();
            }

            await using (MsSqlContext context = NewContext)
            {
                User user = await context.Users.FindAsync("userid");

                user.Username.Should().Be("User");
                user.DateOfBirth.Should().Be(new DateTime(2000, 01, 02));
                user.Bio.Should().Be("new bio");
            }
        }


        [Fact]
        public async Task PutUser_ValidatesRequiredUsername()
        {
            User testUser = new User() { UserId = "userid", Username = "User1", DateOfBirth = DateTime.Today.Date };
            UsersController controller = new UsersController(NewContext);
            var controllerBase = (ControllerBase)controller;
            ActingAs(testUser, ref controllerBase);
            await using (MsSqlContext context = NewContext)
            {
                await context.Users.AddAsync(testUser);

                context.SaveChanges();
            }

            await using (MsSqlContext context = NewContext)
            {
                //This test was modified so that only the actual user with the same id can update user
                var response = await controller.PutUser(new User() { UserId = "userid1", Username = null });

                response.Should().BeOfType<UnauthorizedResult>();
            }

            await using (MsSqlContext context = NewContext)
            {
                User user = await context.Users.FindAsync("userid");

                user.Username.Should().Be("User1");
                user.DateOfBirth.Should().Be(DateTime.Today.Date);
                user.Bio.Should().BeNullOrEmpty();
            }
        }

        [Fact]
        public async Task PostUser_CanCreateAValidUserOfParentType()
        {
            //arrange
            Parent testUser = new Parent() { UserId = "userid", Username = "I am user", DateOfBirth = DateTime.Now.Date, Bio = "I am new user" };
            UsersController controller = new UsersController(NewContext);
            var controllerBase = (ControllerBase)controller;
            ActingAs(testUser, ref controllerBase);
            await using (MsSqlContext context = NewContext)
            {
                var response = await controller.PostUser(testUser);


                response.Result.Should().BeOfType<CreatedAtActionResult>();
            }

            await using (MsSqlContext context = NewContext)
            {
                User user = await context.Users.FindAsync("userid");

                user.Should().NotBeNull();

                user.Username.Should().Be("I am user");
                user.DateOfBirth.Should().Be(DateTime.Now.Date);
                user.Bio.Should().Be("I am new user");
            }
        }

        [Fact]
        public async Task PostUser_PreventsCreationOfUsersWithTheSameID()
        {
            Parent testUser = new Parent() { UserId = "userid", Username = "I am user", DateOfBirth = DateTime.Now.Date, Bio = "I am new user" };
            UsersController controller = new UsersController(NewContext);
            var controllerBase = (ControllerBase)controller;
            ActingAs(testUser, ref controllerBase);
            await using (MsSqlContext context = NewContext)
            {
                await context.Users.AddAsync(testUser);
                context.SaveChanges();
            }

            await using (MsSqlContext context = NewContext)
            { 
                var response = await controller.PostUser(testUser );

                response.Result.Should().BeOfType<ConflictResult>();
            }

            await using (MsSqlContext context = NewContext)
            {
                User user = await context.Users.FindAsync("userid");

                user.Username.Should().Be("I am user");
                user.DateOfBirth.Should().Be(DateTime.Now.Date);
                user.Bio.Should().Be("I am new user");
            }
        }

        [Fact]
        public async Task PostUser_PreventsCreationOfUsersWithMissingRequiredFields()
        {
            Parent testUser = new Parent() { UserId = "userid" };
            UsersController controller = new UsersController(NewContext);
            var controllerBase = (ControllerBase)controller;
            ActingAs(testUser, ref controllerBase);
            await using (MsSqlContext context = NewContext)
            {
                var response = await controller.PostUser(testUser);

                response.Result.Should().BeOfType<BadRequestObjectResult>();
            }

            await using (MsSqlContext context = NewContext)
            {
                User user = await context.Users.FindAsync("userid");
                user.Should().BeNull();
            }
        }

        [Fact]
        public async Task DeleteUser_ShouldRemoveAValidUser()
        {
            User testUser = new User() { UserId = "userid", Username = "I am user", DateOfBirth = DateTime.Today.Date };
            UsersController controller = new UsersController(NewContext);
            var controllerBase = (ControllerBase)controller;
            ActingAs(testUser, ref controllerBase);
            await using (MsSqlContext context = NewContext)
            {
                await context.Users.AddAsync(testUser);

                context.SaveChanges();
            }

            await using (MsSqlContext context = NewContext)
            {

                var response = await controller.DeleteUser("userid");

                response.Should().BeOfType<NoContentResult>();
            }

            await using (MsSqlContext context = NewContext)
            {
                User user = (await context.Users.FindAsync("userid"));
                user.Should().BeNull();
            }
        }

        [Fact] public async Task DeleteUser_CannotRemoveUsersThatDontExist()
        {
            User testUser = new User() { UserId = "userid", Username = "User1", DateOfBirth = DateTime.Today.Date };
            UsersController controller = new UsersController(NewContext);
            var controllerBase = (ControllerBase)controller;
            ActingAs(testUser, ref controllerBase);
            await using MsSqlContext context = NewContext;
            var response = await controller.DeleteUser("userid");
            response.Should().BeOfType<NotFoundResult>();
        }
    }
}
