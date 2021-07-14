using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using webApp.API.Controllers;
using webApp.Test.Utilities;
using FluentAssertions;
using webApp.Models;
using webApp.Data;
using System.Linq;
using Xunit;
using System.Collections.Generic;
using webApp.Resources;

namespace webApp.Test.API.Controllers
{
    public class FamilyControllerTest : DatabaseTest
    {
        [Fact]
        public async Task GetChildren_ReturnsChildrenOfAParent()
        {
            Parent parent = new Parent() { UserId = "parentuserid" };
            FamilyController controller = new FamilyController(NewContext, mapper);
            var controllerBase = (ControllerBase)controller;
            ActingAs(parent, ref controllerBase);
            await using (MsSqlContext context = NewContext)
            {
                await context.Parents.AddAsync(parent);
                await context.Children.AddAsync(new Child() { UserId = "childuserid1", ParentId = "parentuserid" });
                await context.Children.AddAsync(new Child() { UserId = "childuserid2", ParentId = "parentuserid" });

                context.SaveChanges();
            }

            await using (MsSqlContext context = NewContext)
            {
                var result = await controller.GetChildren();
                result.Value.Should().HaveCount(2);
            }
        }

        [Fact]
        public async Task GetChildren_ReturnsNotFoundWhenParentDoesntExist()
        {

            Parent parent = new Parent() { UserId = "parentuserid" };
            FamilyController controller = new FamilyController(NewContext, mapper);
            var controllerBase = (ControllerBase)controller;
            ActingAs(parent, ref controllerBase);
            await using MsSqlContext context = NewContext;
            var result = await controller.GetChildren();

            result.Result.Should().BeOfType<NotFoundResult>();

        }

        [Fact]
        public async Task GetChildren_ReturnAnEmptyListWhenNoChildrenOfParent()
        {
            Parent parent = new Parent() { UserId = "parentuserid" };
            FamilyController controller = new FamilyController(NewContext, mapper);
            var controllerBase = (ControllerBase)controller;
            ActingAs(parent, ref controllerBase);
            await using (MsSqlContext context = NewContext)
            {
                await context.Parents.AddAsync(parent);

                context.SaveChanges();
            }

            await using (MsSqlContext context = NewContext)
            {
                var result = await controller.GetChildren();

                result.Value.Should().HaveCount(0);
            }
        }

        [Fact]
        public async Task GetParent_ReturnsTheParentOfAChild()
        {
            Child child = new Child() { UserId = "childuserid", ParentId = "parentuserid" };
            FamilyController controller = new FamilyController(NewContext, mapper);
            var controllerBase = (ControllerBase)controller;
            ActingAs(child, ref controllerBase);
            await using (MsSqlContext context = NewContext)
            {
                await context.Parents.AddAsync(new Parent() { UserId = "parentuserid2" });
                await context.Parents.AddAsync(new Parent() { UserId = "parentuserid" });
                await context.Children.AddAsync(child);
                context.SaveChanges();
            }

            await using (MsSqlContext context = NewContext)
            {
                var result = await controller.GetParent();

                result.Value.UserId.Should().Be("parentuserid");
            }
        }

        [Fact]
        public async Task CreateChild_CreatesAChildForAParent()
        {
            Parent parent = new Parent() { UserId = "parentid" };
            FamilyController controller = new FamilyController(NewContext, mapper);
            var controllerBase = (ControllerBase)controller;
            ActingAs(parent, ref controllerBase);
            await using (MsSqlContext context = NewContext)
            {
                await context.Parents.AddAsync(parent);

                context.SaveChanges();
            }

            await using (MsSqlContext context = NewContext)
            {
                var result = await controller.CreateChild(new Child() { UserId = "childuserid", Username = "I am a child", ParentId = "parentid" });

                result.Should().BeOfType<NoContentResult>();
            }

            await using (MsSqlContext context = NewContext)
            {
                Child child = await context.Children.FindAsync("childuserid");

                child.Should().NotBeNull();

                child.Username.Should().Be("I am a child");
                child.ParentId.Should().Be("parentid");
            }
        }

        [Fact]
        public async Task CreateChild_ReturnsNotFoundWhenParentDoesntExist()
        {
            Parent parent = new Parent() { UserId = "parentuserid" };
            FamilyController controller = new FamilyController(NewContext, mapper);
            var controllerBase = (ControllerBase)controller;
            ActingAs(parent, ref controllerBase);
            await using (MsSqlContext context = NewContext)
            {
                var result = await controller.CreateChild(new Child() { UserId = "childuserid", Username = "I am a child", ParentId = "parentid" });

                result.Should().BeOfType<NotFoundObjectResult>();
            }

            await using (MsSqlContext context = NewContext)
            {
                Child child = await context.Children.FindAsync("childuserid");
                child.Should().BeNull();
            }
        }

        [Fact]
        public async Task CreateChild_ReturnsUnauthorizedWhenParentIsSomeoneElse()
        {
            Parent parent = new Parent() { UserId = "parentuserid" };
            FamilyController controller = new FamilyController(NewContext, mapper);
            var controllerBase = (ControllerBase)controller;
            ActingAs(parent, ref controllerBase);
            await using (MsSqlContext context = NewContext)
            {
                await context.Parents.AddAsync(parent);

                context.SaveChanges();
            }

            await using (MsSqlContext context = NewContext)
            {
                var result = await controller.CreateChild(new Child() { UserId = "childuserid", Username = "I am a child", ParentId = "some random" });


                result.Should().BeOfType<UnauthorizedObjectResult>();
            }

            await using (MsSqlContext context = NewContext)
            {
                Child child = await context.Children.FindAsync("childuserid");
                child.Should().BeNull();
            }
        }

        [Fact]
        public async Task CreateChild_ReturnBadRequestWhenRequiredFieldsAreLeftOut()
        {
            //arrange
            var parent = new Parent() { UserId = "parentid" };
            FamilyController controller = new FamilyController(NewContext, mapper);
            var controllerBase = (ControllerBase)controller;
            ActingAs(parent, ref controllerBase);
            await using (MsSqlContext context = NewContext)
            {
                await context.Parents.AddAsync(parent);

                context.SaveChanges();
            }

            await using (MsSqlContext context = NewContext)
            {
                // Username missing.
                //act
                var result = await controller.CreateChild(new Child() { UserId = "childuserid", ParentId = "parentid" });

                //assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            await using (MsSqlContext context = NewContext)
            {
                Child child = await context.Children.FindAsync("childuserid");
                child.Should().BeNull();
            }
        }


        [Fact]
        public async Task CreateChild_ReturnsConflictWhenChildExists()
        {
            Parent parent = new Parent() { UserId = "parentuserid" };
            FamilyController controller = new FamilyController(NewContext, mapper);
            var controllerBase = (ControllerBase)controller;
            ActingAs(parent, ref controllerBase);
            await using (MsSqlContext context = NewContext)
            {
                await context.Parents.AddAsync(parent);
                await context.Children.AddAsync(new Child() { UserId = "childuserid", ParentId = "parentuserid" });

                context.SaveChanges();
            }

            await using (MsSqlContext context = NewContext)
            {
                var result = await controller.CreateChild(new Child() { UserId = "childuserid", Username = "I am a child", ParentId = "parentuserid" });

                result.Should().BeOfType<ConflictResult>();
            }

            await using (MsSqlContext context = NewContext)
            {
                context.Children.FirstOrDefault(c => c.Username == "I am a child").Should().BeNull();
            }
        }

        [Fact]
        public async Task GetChildChatLogs_ReturnsChatLogsBetweenChildAndFriend()
        {
            Parent parent = new Parent() { UserId = "parentid" };
            Child child = new Child() { UserId = "childid", ParentId = "parentid" };
            User friend = new User() { UserId = "friendid" };
            Friendship friendship = new Friendship() { UserId = "childid", FriendId = "friendid" };
            Message messageA = new Message() { SenderID = "friendid", ReceiverId = "childid", Content = "Message A" };
            Message messageB = new Message() { SenderID = "childid", ReceiverId = "friendid", Content = "Message B" };

            FamilyController controller = new FamilyController(NewContext, mapper);
            ControllerBase controllerBase = (ControllerBase)controller;
            ActingAs(parent, ref controllerBase);

            await using (MsSqlContext context = NewContext)
            {
                await context.Parents.AddAsync(parent);
                await context.Children.AddAsync(child);
                await context.Users.AddAsync(friend);
                await context.Friendships.AddAsync(friendship);
                await context.Messages.AddAsync(messageA);
                await context.Messages.AddAsync(messageB);

                context.SaveChanges();
            }

            await using (MsSqlContext context1 = NewContext)
            {
                var result = await controller.GetChildChatLogs("childid", "friendid");

                result.Should().BeOfType<ActionResult<IEnumerable<MessageResource>>>();
                result.Value.Should().HaveCount(2);
                result.Value.ToArray()[0].Content.Should().Be(messageA.Content);
                result.Value.ToArray()[1].Content.Should().Be(messageB.Content);
            }
        }

        [Fact]
        public async Task GetChildChatLogs_ReturnsNotFoundIfChildDoesNotExist()
        {
            Parent parent = new Parent() { UserId = "parentid" };
            User friend = new User() { UserId = "friendid" };

            FamilyController controller = new FamilyController(NewContext, mapper);
            ControllerBase controllerBase = (ControllerBase)controller;
            ActingAs(parent, ref controllerBase);

            await using (MsSqlContext context = NewContext)
            {
                await context.Parents.AddAsync(parent);
                await context.Users.AddAsync(friend);

                context.SaveChanges();
            }

            await using (MsSqlContext context1 = NewContext)
            {
                var result = await controller.GetChildChatLogs("childid", "friendid");

                result.Result.Should().BeOfType<NotFoundResult>();
            }
        }

        [Fact]
        public async Task GetChildChatLogs_ReturnsNotFoundIfFriendDoesNotExist()
        {
            Parent parent = new Parent() { UserId = "parentid" };
            Child child = new Child() { UserId = "childid", ParentId = "parentid" };

            FamilyController controller = new FamilyController(NewContext, mapper);
            ControllerBase controllerBase = (ControllerBase)controller;
            ActingAs(parent, ref controllerBase);

            await using (MsSqlContext context = NewContext)
            {
                await context.Parents.AddAsync(parent);
                await context.Children.AddAsync(child);

                context.SaveChanges();
            }

            await using (MsSqlContext context1 = NewContext)
            {
                var result = await controller.GetChildChatLogs("childid", "friendid");

                result.Result.Should().BeOfType<NotFoundResult>();
            }
        }

        [Fact]
        public async Task GetChildChatLogs_ReturnsUnauthorisedIfParentDoesNotBelongToChild()
        {
            Parent actingParent = new Parent() { UserId = "actingparentid" };
            Parent parent = new Parent() { UserId = "parentid" };
            Child child = new Child() { UserId = "childid", ParentId = "parentid" };
            User friend = new User() { UserId = "friendid" };
            Friendship friendship = new Friendship() { UserId = "childid", FriendId = "friendid" };
            Message messageA = new Message() { SenderID = "friendid", ReceiverId = "childid", Content = "Message A" };
            Message messageB = new Message() { SenderID = "childid", ReceiverId = "friendid", Content = "Message B" };

            FamilyController controller = new FamilyController(NewContext, mapper);
            ControllerBase controllerBase = (ControllerBase)controller;
            ActingAs(actingParent, ref controllerBase);

            await using (MsSqlContext context = NewContext)
            {
                await context.Parents.AddAsync(parent);
                await context.Children.AddAsync(child);
                await context.Users.AddAsync(friend);
                await context.Friendships.AddAsync(friendship);
                await context.Messages.AddAsync(messageA);
                await context.Messages.AddAsync(messageB);

                context.SaveChanges();
            }

            await using (MsSqlContext context1 = NewContext)
            {
                var result = await controller.GetChildChatLogs("childid", "friendid");

                result.Result.Should().BeOfType<UnauthorizedResult>();
            }
        }
    }
}
