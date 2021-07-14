using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using webApp.API.Controllers;
using webApp.Test.Utilities;
using webApp.Resources;
using FluentAssertions;
using webApp.Models;
using webApp.Data;
using System.Linq;
using System;
using Xunit;

namespace webApp.Test
{
    public class FriendAddressBookControllerTest : DatabaseTest
    {
        [Fact]
        public async Task AddFriend_ShouldReturn201WhenSucceeds()
        {
            User user = new User() { UserId = "userid", Username = "user", DateOfBirth = new DateTime(1998, 08, 17) };
            User friend = new User() { UserId = "friendid", Username = "friend", DateOfBirth = new DateTime(1998, 07, 17), Email = "friend@mail.com" };

            FriendAddressBookController controller = new FriendAddressBookController(NewContext,mapper);
            var controllerBase = (ControllerBase)controller;
            ActingAs(user, ref controllerBase);

            await using (MsSqlContext context = NewContext)
            {
                await context.Users.AddAsync(user);
                await context.Users.AddAsync(friend);

                context.SaveChanges();
            }

            await using (MsSqlContext context = NewContext)
            {
                var response = await controller.AddFriend("friend@mail.com");

                response.Should().BeOfType<StatusCodeResult>();
                ((StatusCodeResult)response).StatusCode.Should().Equals(201);
            }
        }

        [Fact]
        public async Task AddFriend_ShouldReturnNotFoundWithNonExistingFriend()
        {
            User user = new User() { UserId = "userid", Username = "user", DateOfBirth = new DateTime(1998, 07, 17) };
            FriendAddressBookController controller = new FriendAddressBookController(NewContext, mapper);

            var controllerBase = (ControllerBase)controller;
            ActingAs(user, ref controllerBase);

            await using (MsSqlContext context = NewContext)
            {
                await context.Users.AddAsync(user);

                context.SaveChanges();
            }

            await using (MsSqlContext context = NewContext)
            {
                var response = await controller.AddFriend("friend@mail.com");

                response.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)response).Value.Should().BeEquivalentTo("Friend does not exist.");
            }
        }

        [Fact]
        public async Task AddFriend_ShouldReturnConflictWhenFriendAlreadyAdded()
        {
            User user = new User() { UserId = "userid", Username = "user", DateOfBirth = new DateTime(1998, 08, 17) };
            User friend = new User() { UserId = "friendid", Username = "friend", DateOfBirth = new DateTime(1998, 07, 17), Email = "friend@mail.com" };
            Friendship friendship = new Friendship() { UserId = "userid", FriendId = "friendid" };

            FriendAddressBookController controller = new FriendAddressBookController(NewContext, mapper);
            var controllerBase = (ControllerBase)controller;
            ActingAs(user, ref controllerBase);

            await using (MsSqlContext context = NewContext)
            {
                await context.Users.AddAsync(user);
                await context.Users.AddAsync(friend);
                await context.Friendships.AddAsync(friendship);

                context.SaveChanges();
            }

            await using (MsSqlContext context = NewContext)
            {
                var response = await controller.AddFriend("friend@mail.com");

                response.Should().BeOfType<ConflictObjectResult>();
                ((ConflictObjectResult)response).Value.Should().BeEquivalentTo("User has already added this friend.");
            }
        }

        [Fact]
        public async Task RemoveFriend_ShouldReturnNoContentWhenSucceeds()
        {
            // Arrange
            User user = new User() { UserId = "userid", Username = "user", DateOfBirth = new DateTime(1998, 08, 17) };
            User friend = new User() { UserId = "friendid", Username = "friend", DateOfBirth = new DateTime(1998, 07, 17) };
            Friendship friendship = new Friendship() { UserId = "userid", FriendId = "friendid" };

            FriendAddressBookController controller = new FriendAddressBookController(NewContext, mapper);
            var controllerBase = (ControllerBase)controller;
            ActingAs(user, ref controllerBase);

            await using (MsSqlContext context = NewContext)
            {
                await context.Users.AddAsync(user);
                await context.Users.AddAsync(friend);
                await context.Friendships.AddAsync(friendship);

                context.SaveChanges();
            }

            await using (MsSqlContext context = NewContext)
            {
                var response = await controller.RemoveFriend("friendid");

                response.Should().BeOfType<NoContentResult>();
            }
        }

        [Fact]
        public async Task RemoveFriend_ShouldReturnBadRequestWhenFriendshipDoesntExist()
        {
            User user = new User() { UserId = "userid", Username = "user", DateOfBirth = new DateTime(1998, 08, 17) };
            User friend = new User() { UserId = "friendid", Username = "friend", DateOfBirth = new DateTime(1998, 07, 17) };

            FriendAddressBookController controller = new FriendAddressBookController(NewContext, mapper);
            var controllerBase = (ControllerBase)controller;
            ActingAs(user, ref controllerBase);

            await using (MsSqlContext context = NewContext)
            {
                await context.Users.AddAsync(user);
                await context.Users.AddAsync(friend);

                context.SaveChanges();
            }

            await using (MsSqlContext context = NewContext)
            {
                var response = await controller.RemoveFriend("friendid");

                response.Should().BeOfType<BadRequestObjectResult>();
                ((BadRequestObjectResult)response).Value.Should().BeEquivalentTo("User has not added this friend yet.");
            }
        }

        [Fact]
        public async Task GetFriends_ShouldReturnListOfFriends()
        {
            // Arrange
            User user = new User() { UserId = "userid", Username = "user", DateOfBirth = new DateTime(1998, 08, 17) };
            User friend = new User() { UserId = "friendid", Username = "friend", DateOfBirth = new DateTime(1998, 07, 17) };
            Friendship friendship = new Friendship() { UserId = "userid", FriendId = "friendid" };

            FriendAddressBookController controller = new FriendAddressBookController(NewContext, mapper);
            var controllerBase = (ControllerBase)controller;
            ActingAs(user, ref controllerBase);

            await using (MsSqlContext context = NewContext)
            {
                await context.Users.AddAsync(user);
                await context.Users.AddAsync(friend);
                await context.Friendships.AddAsync(friendship);

                context.SaveChanges();
            }

            await using (MsSqlContext context = NewContext)
            {
                var response = await controller.GetFriends();

                response.Should().BeOfType<List<UserResource>>();
                response.Should().HaveCount(1);
                response.First().UserId.Should().Be("friendid");
            }
        }

        [Fact]
        public async Task GetFriends_ShouldReturnEmptyListWhenNoFriends()
        {
            User user = new User() { UserId = "userid", Username = "user", DateOfBirth = new DateTime(1998, 08, 17) };

            FriendAddressBookController controller = new FriendAddressBookController(NewContext, mapper);
            var controllerBase = (ControllerBase)controller;
            ActingAs(user, ref controllerBase);

            await using (MsSqlContext context = NewContext)
            {
                await context.Users.AddAsync(user);

                context.SaveChanges();
            }

            await using (MsSqlContext context = NewContext)
            {
                var response = await controller.GetFriends();

                response.Should().BeOfType<List<UserResource>>();
                response.Should().BeEmpty();
            }
        }

        [Fact]
        public async Task GetChildFriends_ShouldReturnTheFriendsOfASpecifiedChild()
        {
            Parent parent = new Parent() { UserId = "parentuserid", Username = "Parent", DateOfBirth = new DateTime(1998, 08, 17) };
            Child child = new Child() { UserId = "childuserid", Username = "Child", ParentId = "parentuserid", DateOfBirth = new DateTime(1998, 08, 17) };
            User friend1 = new User() { UserId = "friend1", Username = "Friend 1", DateOfBirth = new DateTime(1998, 08, 17) };
            User friend2 = new User() { UserId = "friend2", Username = "Friend 2", DateOfBirth = new DateTime(1998, 08, 17) };
            Friendship friendship1 = new Friendship() { UserId = "childuserid", FriendId = "friend1"};
            Friendship friendship2 = new Friendship() { UserId = "childuserid", FriendId = "friend2" };

            FriendAddressBookController controller = new FriendAddressBookController(NewContext, mapper);
            var controllerBase = (ControllerBase)controller;
            ActingAs(parent, ref controllerBase);

            await using (MsSqlContext context = NewContext)
            {
                await context.Parents.AddAsync(parent);
                await context.Children.AddAsync(child);
                await context.Users.AddAsync(friend1);
                await context.Users.AddAsync(friend2);
                await context.Friendships.AddAsync(friendship1);
                await context.Friendships.AddAsync(friendship2);

                await context.SaveChangesAsync();
            }

            await using (MsSqlContext context = NewContext)
            {
                var response = controller.GetChildFriends("childuserid");

                response.Result.Should().BeOfType<ActionResult<IEnumerable<UserResource>>>();
                response.Result.Value.Should().HaveCount(2);
                response.Result.Value.ToArray()[0].UserId.Should().Be("friend1");
                response.Result.Value.ToArray()[1].UserId.Should().Be("friend2");
            }
        }

        [Fact]
        public async Task GetChildFriends_ShouldReturnNotFoundIfChildDoesntExist()
        {
            Parent parent = new Parent() { UserId = "parentuserid" };
            User friend1 = new User() { UserId = "friend1" };

            await using (MsSqlContext context = NewContext)
            {
                await context.Parents.AddAsync(parent);
                await context.Users.AddAsync(friend1);
            }

            await using (MsSqlContext context = NewContext)
            {
                FriendAddressBookController controller = new FriendAddressBookController(context, mapper);
                var controllerBase = (ControllerBase)controller;
                ActingAs(parent, ref controllerBase);

                var response = controller.GetChildFriends("childuserid");

                response.Result.Result.Should().BeOfType<NotFoundResult>();
                response.Result.Value.Should().BeNullOrEmpty();
            }
        }

        [Fact]
        public async Task GetChildFriends_ShouldReturnUnauthorisedIfChildDoesNotBelongToActiveUser()
        {
            Parent imposter = new Parent() { UserId = "imposterparentuserid", Username = "Imposter Parent", DateOfBirth = new DateTime(1998, 08, 17) };
            Parent parent = new Parent() { UserId = "parentuserid", Username = "Parent", DateOfBirth = new DateTime(1998, 08, 17) };
            Child child = new Child() { UserId = "childuserid", Username = "Child", ParentId = "parentuserid", DateOfBirth = new DateTime(1998, 08, 17) };
            User friend1 = new User() { UserId = "friend1", Username = "Friend 1", DateOfBirth = new DateTime(1998, 08, 17) };
            User friend2 = new User() { UserId = "friend2", Username = "Friend 2", DateOfBirth = new DateTime(1998, 08, 17) };
            Friendship friendship1 = new Friendship() { UserId = "childuserid", FriendId = "friend1" };
            Friendship friendship2 = new Friendship() { UserId = "childuserid", FriendId = "friend2" };

            FriendAddressBookController controller = new FriendAddressBookController(NewContext, mapper);
            var controllerBase = (ControllerBase)controller;
            ActingAs(imposter, ref controllerBase);

            await using (MsSqlContext context = NewContext)
            {
                await context.Parents.AddAsync(parent);
                await context.Children.AddAsync(child);
                await context.Users.AddAsync(friend1);
                await context.Users.AddAsync(friend2);
                await context.Friendships.AddAsync(friendship1);
                await context.Friendships.AddAsync(friendship2);

                await context.SaveChangesAsync();
            }

            await using (MsSqlContext context = NewContext)
            {
                var response = controller.GetChildFriends("childuserid");

                response.Result.Result.Should().BeOfType<UnauthorizedResult>();
                response.Result.Value.Should().BeNullOrEmpty();
            }
        }
    }
}
