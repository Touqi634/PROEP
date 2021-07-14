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

            BlockController controller = new BlockController(NewContext);
            var controllerBase = (ControllerBase)controller;
            ActingAs(user, ref controllerBase);
            List<User> blockedFriends = new List<User>();
            blockedFriends.Add(friend);

            Friendship friendship = new Friendship() { UserId = userId, FriendId = friendId, IsBlocked = true };


            await using (MsSqlContext context = NewContext)
            {
                await context.Users.AddAsync(user);
                await context.Users.AddAsync(friend);

                await context.Friendships.AddAsync(friendship);

                context.SaveChanges();
            }

            await using (MsSqlContext context = NewContext)
            {
                await controller.BlockUser(friendId);

                // Act
                var response = await controller.GetBlockedUsers();

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
            BlockController controller = new BlockController(NewContext);
            var controllerBase = (ControllerBase)controller;
            ActingAs(user, ref controllerBase);
            List<User> blockedFriends = new List<User>();
            await using (MsSqlContext context = NewContext)
            {
                await context.Users.AddAsync(user);
                await context.Users.AddAsync(friend);

                context.SaveChanges();
            }

            await using (MsSqlContext context = NewContext)
            {
                // Act
                var response = await controller.GetBlockedUsers();

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
            BlockController controller = new BlockController(NewContext);
            var controllerBase = (ControllerBase)controller;
            ActingAs(user, ref controllerBase);
            List<User> blockedFriends = new List<User>();
            Friendship friendship = new Friendship() { UserId = userId, FriendId = friendId };

            await using (MsSqlContext context = NewContext)
            {
                await context.Users.AddAsync(user);
                await context.Users.AddAsync(friend);

                await context.Friendships.AddAsync(friendship);

                context.SaveChanges();
            }

            await using (MsSqlContext context = NewContext)
            {

                // Act
                var response = await controller.BlockUser( friendId);

                // Assert
                response.Should().BeOfType<StatusCodeResult>();
                ((StatusCodeResult)response).StatusCode.Should().Equals(201);
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
            BlockController controller = new BlockController(NewContext);
            var controllerBase = (ControllerBase)controller;
            ActingAs(user, ref controllerBase);
            List<User> blockedFriends = new List<User>();
            await using (MsSqlContext context = NewContext)
            {
                await context.Users.AddAsync(user);

                await context.Friendships.AddAsync(friendship);

                context.SaveChanges();
            }

            await using (MsSqlContext context = NewContext)
            {

                // Act
                var response = await controller.BlockUser(friendId);

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
            BlockController controller = new BlockController(NewContext);
            var controllerBase = (ControllerBase)controller;
            ActingAs(user, ref controllerBase);
            List<User> blockedFriends = new List<User>();
            await using (MsSqlContext context = NewContext)
            {
                await context.Users.AddAsync(user);
                await context.Users.AddAsync(friend);

                context.SaveChanges();
            }

            await using (MsSqlContext context = NewContext)
            {

                // Act
                var response = await controller.BlockUser( friendId);

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
            BlockController controller = new BlockController(NewContext);
            var controllerBase = (ControllerBase)controller;
            ActingAs(user, ref controllerBase);
            List<User> blockedFriends = new List<User>();
            Friendship friendship = new Friendship() { UserId = userId, FriendId = friendId, IsBlocked = true };

            await using (MsSqlContext context = NewContext)
            {
                await context.Users.AddAsync(user);
                await context.Users.AddAsync(friend);

                await context.Friendships.AddAsync(friendship);

                context.SaveChanges();
            }

            await using (MsSqlContext context = NewContext)
            {

                // Act
                var response = await controller.BlockUser( friendId);

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
            Parent parent = new Parent() { UserId = "parent", Username = "userparent", DateOfBirth = new DateTime(1998, 08, 17) };

            Child user = new Child() { UserId = userId, Username = "user",ParentId = parent.UserId, Parent = parent, DateOfBirth = new DateTime(1998, 08, 17) };

            User friend = new User() { UserId = friendId, Username = "friend", DateOfBirth = new DateTime(1998, 07, 17) };
            BlockController controller = new BlockController(NewContext);
            var controllerBase = (ControllerBase)controller;
            ActingAs(parent, ref controllerBase);
            List<User> blockedFriends = new List<User>();
            Friendship friendship = new Friendship() { UserId = userId, FriendId = friendId, IsBlocked = true };

            await using (MsSqlContext context = NewContext)
            {
                await context.Users.AddAsync(user);
                await context.Users.AddAsync(parent);
                await context.Users.AddAsync(friend);

                await context.Friendships.AddAsync(friendship);

                context.SaveChanges();
            }

            await using (MsSqlContext context = NewContext)
            {

                // Act
                var response = await controller.UnblockUser(userId,friendId);

                // Assert
                // It only should pass with the parent of the child being the one making the request

                response.Should().BeOfType<NoContentResult>();
            }
        }

        

        [Theory]
        [InlineData("userid1", "userid2")]
        [InlineData("userid2", "userid3")]
        [InlineData("userid3", "userid4")]
        public async Task UnblockUser_ShouldReturnBadRequestWithNonExistingFriend(string userId, string friendId)
        {
            // Arrange
            Parent parent = new Parent() { UserId = "parent", Username = "userparent", DateOfBirth = new DateTime(1998, 08, 17) };

            Child user = new Child() { UserId = userId, Username = "user", ParentId = parent.UserId, Parent = parent, DateOfBirth = new DateTime(1998, 08, 17) };

            BlockController controller = new BlockController(NewContext);
            var controllerBase = (ControllerBase)controller;
            ActingAs(parent, ref controllerBase);
            List<User> blockedFriends = new List<User>();
            await using (MsSqlContext context = NewContext)
            {
                await context.Users.AddAsync(parent);
                await context.Users.AddAsync(user);

                context.SaveChanges();
            }

            await using (MsSqlContext context = NewContext)
            {
                await controller.BlockUser( friendId);

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
            Parent parent = new Parent() { UserId = "parent", Username = "userparent", DateOfBirth = new DateTime(1998, 08, 17) };

            Child user = new Child() { UserId = userId, Username = "user", ParentId = parent.UserId, Parent = parent, DateOfBirth = new DateTime(1998, 08, 17) };
            User friend = new User() { UserId = friendId, Username = "friend", DateOfBirth = new DateTime(1998, 07, 17) };
            BlockController controller = new BlockController(NewContext);
            var controllerBase = (ControllerBase)controller;
            ActingAs(parent, ref controllerBase);
            List<User> blockedFriends = new List<User>();
            await using (MsSqlContext context = NewContext)
            {
                await context.Users.AddAsync(parent);
                await context.Users.AddAsync(user);
                await context.Users.AddAsync(friend);

                context.SaveChanges();
            }

            await using (MsSqlContext context = NewContext)
            {
                await controller.BlockUser( friendId);

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
            Parent parent = new Parent() { UserId = "parent", Username = "userparent", DateOfBirth = new DateTime(1998, 08, 17) };

            Child user = new Child() { UserId = userId, Username = "user", ParentId = parent.UserId, Parent = parent, DateOfBirth = new DateTime(1998, 08, 17) };
            User friend = new User() { UserId = friendId, Username = "friend", DateOfBirth = new DateTime(1998, 07, 17) };
            BlockController controller = new BlockController(NewContext);
            var controllerBase = (ControllerBase)controller;
            ActingAs(parent, ref controllerBase);
            List<User> blockedFriends = new List<User>();
            Friendship friendship = new Friendship() { UserId = userId, FriendId = friendId };

            await using (MsSqlContext context = NewContext)
            {
                await context.Users.AddAsync(parent);;
                await context.Users.AddAsync(user);;
                await context.Users.AddAsync(friend);

                await context.Friendships.AddAsync(friendship);

                context.SaveChanges();
            }

            await using (MsSqlContext context = NewContext)
            {
                // Act
                var response = await controller.UnblockUser(userId, friendId);

                // Assert
                response.Should().BeOfType<ConflictObjectResult>();
                ((ConflictObjectResult)response).Value.Should().BeEquivalentTo("User has not blocked this friend.");
            }
        }
    }
}
