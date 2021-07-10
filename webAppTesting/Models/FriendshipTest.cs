using Microsoft.EntityFrameworkCore;
using webApp.Test.Utilities;
using FluentAssertions;
using webApp.Models;
using System.Linq;
using webApp.Data;
using System;
using Xunit;
using System.Collections.Generic;

namespace webApp.Test.Models
{
    public class FriendshipTest : DatabaseTest
    {
        [Fact]
        public void CanRetrieveFriendAndUser()
        {
            string userId = "userid1";
            string friendId = "userid2";

            User user = new User() { UserId = userId, Username = "User1", DateOfBirth = new DateTime(1998, 08, 17) };
            User friend = new User() { UserId = friendId, Username = "Friend1", DateOfBirth = new DateTime(1998, 07, 17) };

            using (MsSqlContext context = NewContext)
            {
                context.Users.Add(user);
                context.Users.Add(friend);

                context.Friendships.Add(new Friendship() { UserId = "userid1", FriendId = "userid2" });

                context.SaveChanges();
            }

            using (var context = NewContext)
            {
                Friendship friendship = context.Friendships.Where(f => (f.UserId == userId)).Include(f => f.Friend).Include(f => f.User).Single(f => f.FriendId == friendId);

                friendship.User.Equals(user);
                friendship.User.UserId.Equals(userId);
                friendship.Friend.Equals(friend);
                friendship.Friend.UserId.Equals(friendId);
            }
        }
    }
}
