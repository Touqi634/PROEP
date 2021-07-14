using webApp.Test.Utilities;
using webApp.Models;
using webApp.Data;
using System;
using Xunit;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using FluentAssertions;


namespace webApp.Test.Models
{
    public class FlaggedMessageTest : DatabaseTest
    {
        [Fact]
        public void CanRetrieveSender()
        {
            string receiverId = "user1";
            string senderId = "user2";
            Child receiver = new Child() { UserId = receiverId, Username = "user", DateOfBirth = new DateTime(1998, 08, 17) };
            Child sender = new Child() { UserId = senderId, Username = "user2", DateOfBirth = new DateTime(1998, 07, 17) };

            using (MsSqlContext context = NewContext)
            {
                context.Children.Add(receiver);
                context.Children.Add(sender);
                context.FlaggedMessages.Add(new FlaggedMessage() { FlaggedMessageId = 1, Content = "shit", Reason = "Swear Word", Sender = sender, SenderID = senderId });

                context.SaveChanges();
            }

            using (var context = NewContext)
            {
                FlaggedMessage fm = context.FlaggedMessages.Where(fm => (fm.FlaggedMessageId == 1)).Include(fm => fm.Sender).Single();

                fm.Sender.Equals(sender);
            }
        }
    }
}
