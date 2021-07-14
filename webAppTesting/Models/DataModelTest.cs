using FluentAssertions.Extensions;
using webApp.Test.Utilities;
using FluentAssertions;
using webApp.Models;
using System.Linq;
using webApp.Data;
using System;
using Xunit;

namespace webApp.Test.Models
{
    public class DataModelTest : DatabaseTest
    {
        // User is the most important model inheriting from DataModel so we will use that as our SUT
        User userDataModel = new User() { UserId = "someuserid", Username = "I am a test subject", DateOfBirth = new DateTime(2000, 01, 01) };

        [Fact]
        public void CreatedAt_IsSetWhenObjectIsCreated()
        {

            using (MsSqlContext context = NewContext)
            {
                context.Add(userDataModel);

                context.SaveChanges();
            }

            using (MsSqlContext context = NewContext)
            {
                context.Users.Find("someuserid").CreatedAt.Should().BeWithin(2.Seconds()).Before(DateTime.Now);
            }
        }

        [Fact]
        public void UpdatedAt_IsSetWhenObjectIsChanged()
        {
            using (MsSqlContext context = NewContext)
            {
                context.Add(userDataModel);

                context.SaveChanges();
            }

            using (MsSqlContext context = NewContext)
            {
                User user = context.Users.FirstOrDefault(u => u.UserId == "someuserid");

                user.Bio = "I am a new bio now";

                context.SaveChanges();
            }

            using (MsSqlContext context = NewContext)
            {
                context.Users.FirstOrDefault(u => u.Bio == "I am a new bio now").UpdatedAt.Should().BeWithin(2.Seconds()).Before(DateTime.Now);
            }
        }
    }
}
