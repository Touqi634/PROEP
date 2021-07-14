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
    public class TimeRestrictionTest : DatabaseTest
    {
        [Fact]
        public void CanRetrieveRestrictedUser()
        {
            string childId = "childid1";

            Child child = new Child() { UserId = childId, Username = "User1", DateOfBirth = new DateTime(2014, 08, 17) };

            using (MsSqlContext context = NewContext)
            {
                context.Children.Add(child);
                context.TimeRestrictions.Add(new TimeRestriction
                {
                    RestrictionId = 1,
                    Day = DayOfWeek.Monday,
                    RestrictedUserId = childId,
                    StartTime = new TimeSpan(14, 0, 0),
                    EndTime = new TimeSpan(16, 0, 0)
                });

                context.SaveChanges();
            }

            using (var context = NewContext)
            {
                TimeRestriction tr = context.TimeRestrictions.Where(tr => (tr.RestrictedUserId == childId)).Include(tr => tr.RestrictedUser).Single(tr => tr.RestrictionId == 1);

                tr.RestrictedUser.Equals(child);
                tr.RestrictedUserId.Equals(childId);
            }
        }
    }
}
