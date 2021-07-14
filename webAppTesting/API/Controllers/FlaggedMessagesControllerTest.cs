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
using webApp.Resources;

namespace webApp.Test.API.Controllers
{
    public class FlaggedMessagesControllerTest : DatabaseTest
    {
        [Fact]
        public async Task GetFlaggedMessages_ShouldReturnListOfFlaggedMessages()
        {
            // Arrange
            Parent parent = new Parent() { UserId = "parent", Username = "userparent", DateOfBirth = new DateTime(1960, 07, 12) };

            FlaggedMessagesController controller = new FlaggedMessagesController(NewContext, mapper);
            var controllerBase = (ControllerBase)controller;
            ActingAs(parent, ref controllerBase);

            string receiverId = "user1";
            string senderId = "user2";
            Child receiver = new Child() { UserId = receiverId, Username = "user", DateOfBirth = new DateTime(1998, 08, 17) };
            Child sender = new Child() { UserId = senderId, Username = "user2", ParentId = parent.UserId, Parent = parent, DateOfBirth = new DateTime(1998, 07, 17) };

            FlaggedMessage flaggedMessage = new FlaggedMessage() { FlaggedMessageId = 1, Content = "shit", Reason = "Swear Word", SenderID = senderId };

            UserResource senderUserResource = new UserResource()
            {
                UserId = sender.UserId,
                Bio = sender.Bio,
                DateOfBirth = sender.DateOfBirth,
                Email = sender.Email,
                Phone = sender.Phone,
                Username = sender.Username
            };

            // don't care about receiver in flagged messages
            List<FlaggedMessageResource> flaggedMessages = new List<FlaggedMessageResource>();
            flaggedMessages.Add(new FlaggedMessageResource()
            {
                FlaggedMessageId = 1,
                Content = flaggedMessage.Content,
                Sender = senderUserResource,
                Reason = flaggedMessage.Reason
            });


            using (MsSqlContext context = NewContext)
            {
                context.Parents.Add(parent);
                context.Children.Add(receiver);
                context.Children.Add(sender);
                context.FlaggedMessages.Add(flaggedMessage);

                context.SaveChanges();
            }

            using (MsSqlContext context = NewContext)
            {
                // Act
                var response = await controller.GetFlaggedMessages(senderId);

                // Assert
                response.Should().BeOfType<ActionResult<IEnumerable<FlaggedMessageResource>>>();
                response.Value.Should().BeEquivalentTo(flaggedMessages);
            }
        }

        [Fact]
        public async Task GetFlaggedMessages_ShouldReturnEmptyListIfNoFlaggedMessages()
        {
            // Arrange
            Parent parent = new Parent() { UserId = "parent", Username = "userparent", DateOfBirth = new DateTime(1960, 07, 12) };

            FlaggedMessagesController controller = new FlaggedMessagesController(NewContext, mapper);
            var controllerBase = (ControllerBase)controller;
            ActingAs(parent, ref controllerBase);

            string senderId = "user2";
            Child sender = new Child() { UserId = senderId, Username = "user2", ParentId = parent.UserId, Parent = parent, DateOfBirth = new DateTime(1998, 07, 17) };

            using (MsSqlContext context = NewContext)
            {
                context.Users.Add(sender);

                context.SaveChanges();
            }

            using (MsSqlContext context = NewContext)
            {

                // Act
                var response = await controller.GetFlaggedMessages(senderId);

                // Assert
                response.Should().BeOfType<ActionResult<IEnumerable<FlaggedMessageResource>>>();
                response.Value.Should().BeEmpty();
            }
        }

        [Fact]
        public async Task GetFlaggedMessages_ShouldReturnBadRequestWhenUserDoesNotExist()
        {

            Parent parent = new Parent() { UserId = "parentuserid" };
            FlaggedMessagesController controller = new FlaggedMessagesController(NewContext, mapper);
            var controllerBase = (ControllerBase)controller;
            ActingAs(parent, ref controllerBase);
            await using MsSqlContext context = NewContext;
            var response = await controller.GetFlaggedMessages("childid1");

            response.Result.Should().BeOfType<BadRequestObjectResult>();
            ((BadRequestObjectResult)(response.Result)).Value.Should().BeEquivalentTo("User does not exist.");
        }

        [Fact]
        public async Task PostFlaggedMessage_ShouldReturn204OnSuccess()
        {
            // Arrange
            Parent parent = new Parent() { UserId = "parent", Username = "userparent", DateOfBirth = new DateTime(1960, 07, 12) };

            FlaggedMessagesController controller = new FlaggedMessagesController(NewContext, mapper);
            var controllerBase = (ControllerBase)controller;
            ActingAs(parent, ref controllerBase);

            string senderId = "user2";
            Child sender = new Child() { UserId = senderId, Username = "user2", ParentId = parent.UserId, Parent = parent, DateOfBirth = new DateTime(1998, 07, 17) };

            FlaggedMessage flaggedMessage = new FlaggedMessage() { FlaggedMessageId = 1, Content = "shit", Reason = "Swear Word", SenderID = senderId };

            using (MsSqlContext context = NewContext)
            {
                context.Users.Add(sender);

                context.SaveChanges();
            }

            using (MsSqlContext context = NewContext)
            {

                // Act
                var response = await controller.PostFlaggedMessage(flaggedMessage);

                // Assert
                response.Should().BeOfType<NoContentResult>();
            }
        }

        [Fact]
        public async Task PostFlaggedMessage_ShouldReturnBadRequestWithNullFlaggedMessage()
        {
            // Arrange
            Parent parent = new Parent() { UserId = "parent", Username = "userparent", DateOfBirth = new DateTime(1960, 07, 12) };

            FlaggedMessagesController controller = new FlaggedMessagesController(NewContext, mapper);
            var controllerBase = (ControllerBase)controller;
            ActingAs(parent, ref controllerBase);

            using (MsSqlContext context = NewContext)
            {

                // Act
                var response = await controller.PostFlaggedMessage(null);

                // Assert
                response.Should().BeOfType<BadRequestObjectResult>();
            }
        }
    }
}
