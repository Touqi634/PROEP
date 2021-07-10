using System.Threading.Tasks;
using webApp.API.Controllers;
using webApp.Test.Utilities;
using FluentAssertions;
using webApp.Models;
using webApp.Data;
using System;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace webApp.Test
{
    public class FriendAddressBookControllerTest : DatabaseTest
    {
        [Theory]
        [InlineData("userid1", "userid2")]
        [InlineData("userid2", "userid3")]
        [InlineData("userid3", "userid4")]
        public async Task AddFriend_ShouldReturn201WhenSucceeds(string userId, string friendId)
        {
            // Arrange
            User user = new User() { UserId = userId, Username = "user", DateOfBirth = new DateTime(1998, 08, 17) };
            User friend = new User() { UserId = friendId, Username = "friend", DateOfBirth = new DateTime(1998, 07, 17) };

            using (MsSqlContext context = NewContext)
            {
                context.Users.Add(user);
                context.Users.Add(friend);

                context.SaveChanges();
            }

            using (MsSqlContext context = NewContext)
            {
                FriendAddressBookController controller = new FriendAddressBookController(context);

                // Act
                var response = await controller.AddFriend(userId, friendId);

                // Assert
                response.Should().BeOfType<StatusCodeResult>();
                ((StatusCodeResult)response).StatusCode.Should().Equals(201);
            }
        }

        [Theory]
        [InlineData("userid1", "userid2")]
        [InlineData("userid2", "userid3")]
        [InlineData("userid3", "userid4")]
        public async Task AddFriend_ShouldReturnBadRequestWithNonExistingUser(string userId, string friendId)
        {
            // Arrange
            User friend = new User() { UserId = friendId, Username = "friend", DateOfBirth = new DateTime(1998, 07, 17) };

            using (MsSqlContext context = NewContext)
            {
                context.Users.Add(friend);

                context.SaveChanges();
            }

            using (MsSqlContext context = NewContext)
            {
                FriendAddressBookController controller = new FriendAddressBookController(context);

                // Act
                var response = await controller.AddFriend(userId, friendId);

                // Assert
                response.Should().BeOfType<BadRequestObjectResult>();
                ((BadRequestObjectResult)response).Value.Should().BeEquivalentTo("User does not exist.");
            }
        }

        [Theory]
        [InlineData("userid1", "userid2")]
        [InlineData("userid2", "userid3")]
        [InlineData("userid3", "userid4")]
        public async Task AddFriend_ShouldReturnNotFoundWithNonExistingFriend(string userId, string friendId)
        {
            // Arrange
            User user = new User() { UserId = userId, Username = "friend", DateOfBirth = new DateTime(1998, 07, 17) };

            using (MsSqlContext context = NewContext)
            {
                context.Users.Add(user);

                context.SaveChanges();
            }

            using (MsSqlContext context = NewContext)
            {
                FriendAddressBookController controller = new FriendAddressBookController(context);

                // Act
                var response = await controller.AddFriend(userId, friendId);

                // Assert
                response.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)response).Value.Should().BeEquivalentTo("Friend does not exist.");
            }
        }

        [Fact]
        public async Task AddFriend_ShouldReturnConflictWhenFriendAlreadyAdded()
        {
            // Arrange
            string userId = "userid1";
            string friendId = "userid2";
            User user = new User() { UserId = userId, Username = "user", DateOfBirth = new DateTime(1998, 08, 17) };
            User friend = new User() { UserId = friendId, Username = "friend", DateOfBirth = new DateTime(1998, 07, 17) };

            using (MsSqlContext context = NewContext)
            {
                context.Users.Add(user);
                context.Users.Add(friend);

                context.SaveChanges();
            }

            using (MsSqlContext context = NewContext)
            {
                FriendAddressBookController controller = new FriendAddressBookController(context);

                await controller.AddFriend(userId, friendId);

                // Act & Assert
                var response = await controller.AddFriend(userId, friendId);

                // Assert
                response.Should().BeOfType<ConflictObjectResult>();
                ((ConflictObjectResult)response).Value.Should().BeEquivalentTo("User has already added this friend.");
            }
        }

        [Theory]
        [InlineData("userid1", "userid2")]
        [InlineData("userid2", "userid3")]
        [InlineData("userid3", "userid4")]
        public async Task RemoveFriend_ShouldReturnNoContentWhenSucceeds(string userId, string friendId)
        {
            // Arrange
            User user = new User() { UserId = userId, Username = "user", DateOfBirth = new DateTime(1998, 08, 17) };
            User friend = new User() { UserId = friendId, Username = "friend", DateOfBirth = new DateTime(1998, 07, 17) };

            using (MsSqlContext context = NewContext)
            {
                context.Users.Add(user);
                context.Users.Add(friend);

                context.SaveChanges();
            }

            using (MsSqlContext context = NewContext)
            {
                FriendAddressBookController controller = new FriendAddressBookController(context);

                await controller.AddFriend(userId, friendId);

                // Act
                var response = await controller.RemoveFriend(userId, friendId);

                // Assert
                response.Should().BeOfType<NoContentResult>();
            }
        }

        [Theory]
        [InlineData("userid1", "userid2")]
        [InlineData("userid2", "userid3")]
        [InlineData("userid3", "userid4")]
        public async Task RemoveFriend_ShouldReturnBadRequestWhenFriendshipDoesntExist(string userId, string friendId)
        {
            // Arrange
            User user = new User() { UserId = userId, Username = "user", DateOfBirth = new DateTime(1998, 08, 17) };
            User friend = new User() { UserId = friendId, Username = "friend", DateOfBirth = new DateTime(1998, 07, 17) };

            using (MsSqlContext context = NewContext)
            {
                context.Users.Add(user);
                context.Users.Add(friend);

                context.SaveChanges();
            }

            using (MsSqlContext context = NewContext)
            {
                FriendAddressBookController controller = new FriendAddressBookController(context);

                // Act
                var response = await controller.RemoveFriend(userId, friendId);

                // Assert
                response.Should().BeOfType<BadRequestObjectResult>();
                ((BadRequestObjectResult)response).Value.Should().BeEquivalentTo("User has not added this friend yet.");
            }
        }

        [Theory]
        [InlineData("userid1", "userid2")]
        [InlineData("userid2", "userid3")]
        [InlineData("userid3", "userid4")]
        public async Task GetFriends_ShouldReturnListOfFriends(string userId, string friendId)
        {
            // Arrange
            User user = new User() { UserId = userId, Username = "user", DateOfBirth = new DateTime(1998, 08, 17) };
            User friend = new User() { UserId = friendId, Username = "friend", DateOfBirth = new DateTime(1998, 07, 17) };
            List<User> friends = new List<User>();
            friends.Add(friend);

            using (MsSqlContext context = NewContext)
            {
                context.Users.Add(user);
                context.Users.Add(friend);

                context.SaveChanges();
            }

            using (MsSqlContext context = NewContext)
            {
                FriendAddressBookController controller = new FriendAddressBookController(context);

                await controller.AddFriend(userId, friendId);

                // Act
                var response = await controller.GetFriends(userId);

                // Assert
                response.Should().BeOfType<ActionResult<IEnumerable<User>>>();
                response.Value.Should().BeEquivalentTo(friends);
            }
        }

        [Theory]
        [InlineData("userid1")]
        [InlineData("userid2")]
        [InlineData("userid3")]
        public async Task GetFriends_ShouldReturnEmptyListWhenNoFriends(string userId)
        {
            // Arrange
            User user = new User() { UserId = userId, Username = "user", DateOfBirth = new DateTime(1998, 08, 17) };

            using (MsSqlContext context = NewContext)
            {
                context.Users.Add(user);

                context.SaveChanges();
            }

            using (MsSqlContext context = NewContext)
            {
                FriendAddressBookController controller = new FriendAddressBookController(context);

                // Act
                var response = await controller.GetFriends(userId);

                // Assert
                response.Should().BeOfType<ActionResult<IEnumerable<User>>>();
                response.Value.Should().BeEmpty();
            }
        }

        [Theory]
        [InlineData("userid1")]
        [InlineData("userid2")]
        [InlineData("userid3")]
        public async Task GetFriends_ShouldReturnEmptyListWhenUserDoesNotExist(string userId)
        {
            // Arrange
            using (MsSqlContext context = NewContext)
            {
                context.SaveChanges();
            }

            using (MsSqlContext context = NewContext)
            {
                FriendAddressBookController controller = new FriendAddressBookController(context);

                // Act
                var response = await controller.GetFriends(userId);

                // Assert
                response.Should().BeOfType<ActionResult<IEnumerable<User>>>();
                response.Value.Should().BeEmpty();
            }
        }
    }
}
