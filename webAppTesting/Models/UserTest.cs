using Microsoft.EntityFrameworkCore;
using webApp.Test.Utilities;
using FluentAssertions;
using webApp.Models;
using System.Linq;
using webApp.Data;
using System;
using Xunit;

namespace webApp.Test.Models
{
    public class UserTest : DatabaseTest
    {
        [Fact]
        public void Username_IsRequired()
        {
            User SUT = new User { Username = null };

            Validate.Model(SUT).Any(
                    v => v.MemberNames.Contains("Username") && v.ErrorMessage.Contains("required")
                ).Should().BeTrue();
        }

        [Fact]
        public void CanHaveManyFriends()
        {
            using (MsSqlContext context = NewContext)
            {
                context.Users.Add( new User() { UserId = "userid1", Username = "User1", DateOfBirth = DateTime.Today.Date });
                context.Users.Add( new User() { UserId = "userid2", Username = "Friend1", DateOfBirth = DateTime.Today.Date });
                context.Users.Add( new User() { UserId = "userid3", Username = "Friend2", DateOfBirth = DateTime.Today.Date });

                context.Friendships.Add( new Friendship() { UserId = "userid1", FriendId = "userid2" });
                context.Friendships.Add(new Friendship() { UserId = "userid1", FriendId = "userid3" });

                context.SaveChanges();
            }

            using (var context = NewContext)
            {
                var testUser = context.Users.Include(u => u.Friends)
                    .ThenInclude(f => f.Friend)
                    .Single(u => u.UserId == "userid1");

                testUser.Friends.Should().HaveCount(2);

                testUser.Friends.Where(u => u.Friend.Username == "Friend1").Should().HaveCount(1);
                testUser.Friends.Where(u => u.Friend.Username == "Friend2").Should().HaveCount(1);
            }
        }
    }
}
