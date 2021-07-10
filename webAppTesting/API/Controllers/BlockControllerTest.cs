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

namespace webApp.Test.API.Controllers
{
    public class BlockControllerTest : DatabaseTest
    {
        [Theory]
        [InlineData("userid1", "userid2")]
        [InlineData("userid2", "userid3")]
        [InlineData("userid3", "userid4")]
        public async Task GetBlockedUsers_ShouldReturnListOfBlockedUsers(string userId, string friendId)
        {
            // Arrange
            User user = new User() { UserId = userId, Username = "user", DateOfBirth = new DateTime(1998, 08, 17) };
            User friend = new User() { UserId = friendId, Username = "friend", DateOfBirth = new DateTime(1998, 07, 17) };

            List<User> blockedFriends = new List<User>();
            blockedFriends.Add(friend);

            Friendship friendship = new Friendship() { UserId = userId, FriendId = friendId, IsBlocked = true };
            

            using (MsSqlContext context = NewContext)
            {
                context.Users.Add(user);
                context.Users.Add(friend);

                context.Friendships.Add(friendship);

                context.SaveChanges();
            }

            using (MsSqlContext context = NewContext)
            {
                BlockController controller = new BlockController(context);

                await controller.BlockUser(userId, friendId);

                // Act
                var response = await controller.GetBlockedUsers(userId);

                // Assert
                response.Should().BeOfType<ActionResult<IEnumerable<User>>>();
                response.Value.Should().BeEquivalentTo(blockedFriends);
            }
        }

        [Theory]
        [InlineData("userid1", "userid2")]
        [InlineData("userid2", "userid3")]
        [InlineData("userid3", "userid4")]
        public async Task GetBlockedUsers_ShouldReturnEmptyListIfNoBlockedUsers(string userId, string friendId)
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
                BlockController controller = new BlockController(context);

                // Act
                var response = await controller.GetBlockedUsers(userId);

                // Assert
                response.Should().BeOfType<ActionResult<IEnumerable<User>>>();
                response.Value.Should().BeEmpty();
            }
        }

        [Theory]
        [InlineData("userid1")]
        [InlineData("userid2")]
        [InlineData("userid3")]
        public async Task GetBlockedUsers_ShouldReturnEmptyListWhenUserDoesNotExist(string userId)
        {
            // Arrange
            using (MsSqlContext context = NewContext)
            {
                context.SaveChanges();
            }

            using (MsSqlContext context = NewContext)
            {
                BlockController controller = new BlockController(context);

                // Act
                var response = await controller.GetBlockedUsers(userId);

                // Assert
                response.Should().BeOfType<ActionResult<IEnumerable<User>>>();
                response.Value.Should().BeEmpty();
            }
        }

        [Theory]
        [InlineData("userid1", "userid2")]
        [InlineData("userid2", "userid3")]
        [InlineData("userid3", "userid4")]
        public async Task BlockUser_ShouldReturn201WhenSucceeds(string userId, string friendId)
        {
            // Arrange
            User user = new User() { UserId = userId, Username = "user", DateOfBirth = new DateTime(1998, 08, 17) };
            User friend = new User() { UserId = friendId, Username = "friend", DateOfBirth = new DateTime(1998, 07, 17) };

            Friendship friendship = new Friendship() { UserId = userId, FriendId = friendId };

            using (MsSqlContext context = NewContext)
            {
                context.Users.Add(user);
                context.Users.Add(friend);

                context.Friendships.Add(friendship);

                context.SaveChanges();
            }

            using (MsSqlContext context = NewContext)
            {
                BlockController controller = new BlockController(context);
                
                // Act
                var response = await controller.BlockUser(userId, friendId);

                // Assert
                response.Should().BeOfType<StatusCodeResult>();
                ((StatusCodeResult)response).StatusCode.Should().Equals(201);
            }
        }

        [Theory]
        [InlineData("userid1", "userid2")]
        [InlineData("userid2", "userid3")]
        [InlineData("userid3", "userid4")]
        public async Task BlockUser_ShouldReturnBadRequestWithNonExistingUser(string userId, string friendId)
        {
            // Arrange
            User friend = new User() { UserId = friendId, Username = "friend", DateOfBirth = new DateTime(1998, 07, 17) };
            Friendship friendship = new Friendship() { UserId = userId, FriendId = friendId, IsBlocked = true };

            using (MsSqlContext context = NewContext)
            {
                context.Users.Add(friend);

                context.Friendships.Add(friendship);

                context.SaveChanges();
            }

            using (MsSqlContext context = NewContext)
            {
                BlockController controller = new BlockController(context);

                // Act
                var response = await controller.BlockUser(userId, friendId);

                // Assert
                response.Should().BeOfType<BadRequestObjectResult>();
                ((BadRequestObjectResult)response).Value.Should().BeEquivalentTo("User does not exist.");
            }
        }

        [Theory]
        [InlineData("userid1", "userid2")]
        [InlineData("userid2", "userid3")]
        [InlineData("userid3", "userid4")]
        public async Task BlockUser_ShouldReturnBadRequestWithNonExistingFriend(string userId, string friendId)
        {
            // Arrange
            User user = new User() { UserId = userId, Username = "friend", DateOfBirth = new DateTime(1998, 07, 17) };
            Friendship friendship = new Friendship() { UserId = userId, FriendId = friendId, IsBlocked = true };

            using (MsSqlContext context = NewContext)
            {
                context.Users.Add(user);

                context.Friendships.Add(friendship);

                context.SaveChanges();
            }

            using (MsSqlContext context = NewContext)
            {
                BlockController controller = new BlockController(context);

                // Act
                var response = await controller.BlockUser(userId, friendId);

                // Assert
                response.Should().BeOfType<BadRequestObjectResult>();
                ((BadRequestObjectResult)response).Value.Should().BeEquivalentTo("Friend does not exist.");
            }
        }

        [Theory]
        [InlineData("userid1", "userid2")]
        [InlineData("userid2", "userid3")]
        [InlineData("userid3", "userid4")]
        public async Task BlockUser_ShouldReturnNotFoundWhenFriendshipDoesntExist(string userId, string friendId)
        {
            // Arrange
            User user = new User() { UserId = userId, Username = "friend", DateOfBirth = new DateTime(1998, 07, 17) };
            User friend = new User() { UserId = friendId, Username = "friend", DateOfBirth = new DateTime(1998, 07, 17) };

            using (MsSqlContext context = NewContext)
            {
                context.Users.Add(user);
                context.Users.Add(friend);

                context.SaveChanges();
            }

            using (MsSqlContext context = NewContext)
            {
                BlockController controller = new BlockController(context);

                // Act
                var response = await controller.BlockUser(userId, friendId);

                // Assert
                response.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)response).Value.Should().BeEquivalentTo("The specified friendship does not exist.");
            }
        }

        [Theory]
        [InlineData("userid1", "userid2")]
        [InlineData("userid2", "userid3")]
        [InlineData("userid3", "userid4")]
        public async Task BlockUser_ShouldReturnConflictWhenFriendshipAlreadyBlocked(string userId, string friendId)
        {
            // Arrange
            User user = new User() { UserId = userId, Username = "friend", DateOfBirth = new DateTime(1998, 07, 17) };
            User friend = new User() { UserId = friendId, Username = "friend", DateOfBirth = new DateTime(1998, 07, 17) };

            Friendship friendship = new Friendship() { UserId = userId, FriendId = friendId, IsBlocked = true };

            using (MsSqlContext context = NewContext)
            {
                context.Users.Add(user);
                context.Users.Add(friend);

                context.Friendships.Add(friendship);

                context.SaveChanges();
            }

            using (MsSqlContext context = NewContext)
            {
                BlockController controller = new BlockController(context);

                // Act
                var response = await controller.BlockUser(userId, friendId);

                // Assert
                response.Should().BeOfType<ConflictObjectResult>();
                ((ConflictObjectResult)response).Value.Should().BeEquivalentTo("User has already blocked this friend.");
            }
        }

        [Theory]
        [InlineData("userid1", "userid2")]
        [InlineData("userid2", "userid3")]
        [InlineData("userid3", "userid4")]
        public async Task UnblockUser_ShouldReturnNoContentWhenSucceeds(string userId, string friendId)
        {
            // Arrange
            User user = new User() { UserId = userId, Username = "user", DateOfBirth = new DateTime(1998, 08, 17) };
            User friend = new User() { UserId = friendId, Username = "friend", DateOfBirth = new DateTime(1998, 07, 17) };

            Friendship friendship = new Friendship() { UserId = userId, FriendId = friendId, IsBlocked = true };

            using (MsSqlContext context = NewContext)
            {
                context.Users.Add(user);
                context.Users.Add(friend);

                context.Friendships.Add(friendship);

                context.SaveChanges();
            }

            using (MsSqlContext context = NewContext)
            {
                BlockController controller = new BlockController(context);

                // Act
                var response = await controller.UnblockUser(userId, friendId);

                // Assert
                response.Should().BeOfType<NoContentResult>();
            }
        }

        [Theory]
        [InlineData("userid1", "userid2")]
        [InlineData("userid2", "userid3")]
        [InlineData("userid3", "userid4")]
        public async Task UnblockUser_ShouldReturnBadRequestWithNonExistingUser(string userId, string friendId)
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
                BlockController controller = new BlockController(context);

                // Act
                var response = await controller.UnblockUser(userId, friendId);

                // Assert
                response.Should().BeOfType<BadRequestObjectResult>();
                ((BadRequestObjectResult)response).Value.Should().BeEquivalentTo("User does not exist.");
            }
        }

        [Theory]
        [InlineData("userid1", "userid2")]
        [InlineData("userid2", "userid3")]
        [InlineData("userid3", "userid4")]
        public async Task UnblockUser_ShouldReturnBadRequestWithNonExistingFriend(string userId, string friendId)
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
                BlockController controller = new BlockController(context);

                await controller.BlockUser(userId, friendId);

                // Act
                var response = await controller.UnblockUser(userId, friendId);

                // Assert
                response.Should().BeOfType<BadRequestObjectResult>();
                ((BadRequestObjectResult)response).Value.Should().BeEquivalentTo("Friend does not exist.");
            }
        }

        [Theory]
        [InlineData("userid1", "userid2")]
        [InlineData("userid2", "userid3")]
        [InlineData("userid3", "userid4")]
        public async Task UnblockUser_ShouldReturnNotFoundWhenFriendshipDoesntExist(string userId, string friendId)
        {
            // Arrange
            User user = new User() { UserId = userId, Username = "friend", DateOfBirth = new DateTime(1998, 07, 17) };
            User friend = new User() { UserId = friendId, Username = "friend", DateOfBirth = new DateTime(1998, 07, 17) };

            using (MsSqlContext context = NewContext)
            {
                context.Users.Add(user);
                context.Users.Add(friend);

                context.SaveChanges();
            }

            using (MsSqlContext context = NewContext)
            {
                BlockController controller = new BlockController(context);
                await controller.BlockUser(userId, friendId);

                // Act
                var response = await controller.UnblockUser(userId, friendId);

                // Assert
                response.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)response).Value.Should().BeEquivalentTo("The specified friendship does not exist.");
            }
        }

        [Theory]
        [InlineData("userid1", "userid2")]
        [InlineData("userid2", "userid3")]
        [InlineData("userid3", "userid4")]
        public async Task UnblockUser_ShouldReturnConflictWhenFriendshipNotBlocked(string userId, string friendId)
        {
            // Arrange
            User user = new User() { UserId = userId, Username = "friend", DateOfBirth = new DateTime(1998, 07, 17) };
            User friend = new User() { UserId = friendId, Username = "friend", DateOfBirth = new DateTime(1998, 07, 17) };

            Friendship friendship = new Friendship() { UserId = userId, FriendId = friendId };

            using (MsSqlContext context = NewContext)
            {
                context.Users.Add(user);
                context.Users.Add(friend);

                context.Friendships.Add(friendship);

                context.SaveChanges();
            }

            using (MsSqlContext context = NewContext)
            {
                BlockController controller = new BlockController(context);

                // Act
                var response = await controller.UnblockUser(userId, friendId);

                // Assert
                response.Should().BeOfType<ConflictObjectResult>();
                ((ConflictObjectResult)response).Value.Should().BeEquivalentTo("User has not blocked this friend.");
            }
        }
    }
}
