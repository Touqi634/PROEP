using webApp.API.Controllers;
using webApp.Test.Utilities;
using FluentAssertions;
using webApp.Models;
using webApp.Data;
using System;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using webApp.Resources;
using AutoMapper;
using webApp.Mapping;

namespace webApp.Test.API.Controllers
{
    public class TimeRestrictionsControllerTest : DatabaseTest
    {
        [Fact]
        public async Task GetTimeRestrictions_ShouldReturnListOfTimeRestrictions()
        {
            string childId = "childuserid1";
            Parent parent = new() { UserId = "parentuserid" };
            Child child = new() { UserId = childId, ParentId = "parentuserid" };
            TimeRestrictionsController controller = new(NewContext, mapper);
            var controllerBase = (ControllerBase)controller;
            ActingAs(parent, ref controllerBase);
            List<TimeRestrictionResource> expectedTimeRestrictionResources = new();
            expectedTimeRestrictionResources.Add(new TimeRestrictionResource()
            {
                Day = DayOfWeek.Tuesday,
                StartTime = new TimeSpan(0, 0, 0),
                EndTime = new TimeSpan(07, 30, 0)
            });
            expectedTimeRestrictionResources.Add(new TimeRestrictionResource()
            {
                Day = DayOfWeek.Tuesday,
                StartTime = new TimeSpan(0, 0, 0),
                EndTime = new TimeSpan(07, 30, 0)
            });

            // Arrange
            await using (MsSqlContext context = NewContext)
            {
                await context.Parents.AddAsync(parent);
                await context.Children.AddAsync(child);
                await context.TimeRestrictions.AddAsync(new TimeRestriction()
                {
                    RestrictedUserId = childId,
                    Day = DayOfWeek.Tuesday,
                    StartTime = new TimeSpan(0, 0, 0),
                    EndTime = new TimeSpan(07, 30, 0)
                });
                await context.TimeRestrictions.AddAsync(new TimeRestriction()
                {
                    RestrictedUserId = childId,
                    Day = DayOfWeek.Tuesday,
                    StartTime = new TimeSpan(0, 0, 0),
                    EndTime = new TimeSpan(07, 30, 0)
                });
                await context.TimeRestrictions.AddAsync(new TimeRestriction()
                {
                    RestrictedUserId = "otherchild",
                    Day = DayOfWeek.Tuesday,
                    StartTime = new TimeSpan(0, 0, 0),
                    EndTime = new TimeSpan(07, 30, 0)
                });

                context.SaveChanges();
            }

            await using (MsSqlContext context = NewContext)
            {

                // Act
                var response = await controller.GetTimeRestrictions(childId);

                // Assert
                response.Should().BeOfType<ActionResult<IEnumerable<TimeRestrictionResource>>>();
                response.Value.Should().BeEquivalentTo(expectedTimeRestrictionResources);
            }
        }

        [Fact]
        public async Task AddTimeRestriction_ShouldReturnBadRequestWithNonExistingChild()
        {
            string childId = "childuserid1";
            Parent parent = new() { UserId = "parentuserid" };
            TimeRestrictionsController controller = new(NewContext, mapper);
            var controllerBase = (ControllerBase)controller;
            ActingAs(parent, ref controllerBase);
            // Arrange
            await using (MsSqlContext context = NewContext)
            {
                context.SaveChanges();
            }

            await using (MsSqlContext context = NewContext)
            {

                // Act
                var response = await controller.AddTimeRestriction(new TimeRestriction()
                {
                    RestrictedUserId = childId,
                    Day = DayOfWeek.Tuesday,
                    StartTime = new TimeSpan(0, 0, 0),
                    EndTime = new TimeSpan(07, 30, 0)
                });

                // Assert
                response.Should().BeOfType<BadRequestObjectResult>();
                ((BadRequestObjectResult)response).Value.Should().BeEquivalentTo("Child does not exist.");
            }
        }

        [Fact]
        public async Task AddTimeRestriction_ShouldReturn201WhenSucceeds()
        {
            string childId = "childuserid1";
            Parent parent = new() { UserId = "parentuserid" };
            Child child = new() { UserId = childId, ParentId = "parentuserid" };
            TimeRestrictionsController controller = new(NewContext,mapper);
            var controllerBase = (ControllerBase)controller;
            ActingAs(parent, ref controllerBase);
            List<User> blockedFriends = new();


            // Arrange
            await using (MsSqlContext context = NewContext)
            {
                await context.Parents.AddAsync(parent);
                await context.Children.AddAsync(child);
                
                context.SaveChanges();
            }

            await using (MsSqlContext context = NewContext)
            {
                // Act
                var response = await controller.AddTimeRestriction(new TimeRestriction()
                {
                    RestrictedUserId = childId,
                    Day = DayOfWeek.Tuesday,
                    StartTime = new TimeSpan(0, 0, 0),
                    EndTime = new TimeSpan(07, 30, 0)
                });

                // Assert
                response.Should().BeOfType<StatusCodeResult>();
                ((StatusCodeResult)response).StatusCode.Should().Equals(201);
            }
        }

        [Fact]
        public async Task UpdateTimeRestriction_ShouldReturnBadRequestWithNonMatchingRestrictionIds()
        {
            string childId = "childuserid1";
            Parent parent = new() { UserId = "parentuserid" };
            Child child = new() { UserId = childId, ParentId = "parentuserid" };
            TimeRestrictionsController controller = new(NewContext, mapper);
            var controllerBase = (ControllerBase)controller;
            ActingAs(parent, ref controllerBase);
            TimeRestriction timeRestriction = new()
            {
                RestrictionId = 1,
                RestrictedUserId = childId,
                Day = DayOfWeek.Tuesday,
                StartTime = new TimeSpan(0, 0, 0),
                EndTime = new TimeSpan(07, 30, 0)
            };

            // Arrange
            await using (MsSqlContext context = NewContext)
            {
                await context.Parents.AddAsync(parent);
                await context.Children.AddAsync(child);
                await context.TimeRestrictions.AddAsync(timeRestriction);

                context.SaveChanges();
            }

            await using (MsSqlContext context = NewContext)
            {

                timeRestriction.Day = DayOfWeek.Wednesday;

                // Act
                var response = await controller.UpdateTimeRestriction(2, timeRestriction);

                // Assert
                response.Should().BeOfType<BadRequestResult>();
            }
        }

        [Fact]
        public async Task UpdateTimeRestriction_ShouldReturnNotFoundWithNonExistingRestrictionId()
        {
            string childId = "childuserid1"; 
            Parent parent = new() { UserId = "parentuserid" };
            Child child = new() { UserId = childId, ParentId = "parentuserid" };
            TimeRestrictionsController controller = new(NewContext, mapper);
            var controllerBase = (ControllerBase)controller;
            ActingAs(parent, ref controllerBase);
            TimeRestriction timeRestriction = new()
            {
                RestrictionId = 1,
                RestrictedUserId = childId,
                RestrictedUser = child,
                Day = DayOfWeek.Tuesday,
                StartTime = new TimeSpan(0, 0, 0),
                EndTime = new TimeSpan(07, 30, 0)
            };

            // Arrange
            await using (MsSqlContext context = NewContext)
            {
                context.SaveChanges();
            }

            await using (MsSqlContext context = NewContext)
            {

                // Act
                var response = await controller.UpdateTimeRestriction(1, timeRestriction);

                // Assert
                response.Should().BeOfType<NotFoundResult>();
            }
        }

        [Fact]
        public async Task UpdateTimeRestriction_ShouldReturnNoContentWhenSucceeds()
        {
            string childId = "childuserid1";
            Parent parent = new() { UserId = "parentuserid" };
            Child child = new() { UserId = childId, ParentId = "parentuserid" };
            TimeRestrictionsController controller = new(NewContext, mapper);
            var controllerBase = (ControllerBase)controller;
            ActingAs(parent, ref controllerBase);
            TimeRestriction timeRestriction = new()
            {
                RestrictionId = 1,
                RestrictedUserId = childId,
                Day = DayOfWeek.Tuesday,
                StartTime = new TimeSpan(0, 0, 0),
                EndTime = new TimeSpan(07, 30, 0)
            };

            // Arrange
            await using (MsSqlContext context = NewContext)
            {
                await context.Parents.AddAsync(parent);
                await context.Children.AddAsync(child);
                await context.TimeRestrictions.AddAsync(timeRestriction);

                context.SaveChanges();
            }

            await using (MsSqlContext context = NewContext)
            {

                timeRestriction.Day = DayOfWeek.Wednesday;

                // Act
                var response = await controller.UpdateTimeRestriction(1, timeRestriction);

                // Assert
                response.Should().BeOfType<NoContentResult>();
            }
        }

        [Fact]
        public async Task DeleteTimeRestriction_ShouldReturnNotFoundWithNonExistingRestrictionId()
        {
            Parent parent = new() { UserId = "parentuserid" };
            TimeRestrictionsController controller = new(NewContext, mapper);
            var controllerBase = (ControllerBase)controller;
            ActingAs(parent, ref controllerBase);
            // Arrange
            await using (MsSqlContext context = NewContext)
            {
                context.SaveChanges();
            }

            await using (MsSqlContext context = NewContext)
            {

                // Act
                var response = await controller.DeleteTimeRestriction(1);

                // Assert
                response.Should().BeOfType<NotFoundResult>();
            }
        }

        [Fact]
        public async Task DeleteTimeRestriction_ShouldReturnNoContentWhenSucceeds()
        {
            string childId = "childuserid1";
            Parent parent = new() { UserId = "parentuserid" };
            Child child = new() { UserId = childId, ParentId = "parentuserid"};
            
            TimeRestrictionsController controller = new(NewContext, mapper);
            var controllerBase = (ControllerBase)controller;
            ActingAs(parent, ref controllerBase);
            // Arrange
            TimeRestriction restriction = new()
            {
                RestrictionId = 1,
                RestrictedUserId = childId,
                RestrictedUser = child,
                Day = DayOfWeek.Tuesday,
                StartTime = new TimeSpan(0, 0, 0),
                EndTime = new TimeSpan(07, 30, 0)
            };
            await using (MsSqlContext context = NewContext)
            {
                await context.Parents.AddAsync(parent);
                await context.Children.AddAsync(child);
                
                await context.TimeRestrictions.AddAsync(restriction);
                
                context.SaveChanges();
            }

            await using (MsSqlContext context = NewContext)
            {
                // Act
                var response = await controller.DeleteTimeRestriction(1);

                // Assert
                response.Should().BeOfType<NoContentResult>();
            }
        }
    }
}
