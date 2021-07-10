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

            using (MsSqlContext context = NewContext)
            {
                context.Users.Add(testUser);

                context.SaveChanges();
            }

            using (MsSqlContext context = NewContext)
            {
                UsersController controller = new UsersController(context);

                var response = await controller.GetUser("userid1");

                response.Value.Should().BeOfType<User>();

                response.Value.Should().BeEquivalentTo(testUser);
            }
        }
    }
}
