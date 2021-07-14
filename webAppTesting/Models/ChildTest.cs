using System.Collections.Generic;
using System.Threading.Tasks;
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
    public class ChildTest : DatabaseTest
    {
        [Fact]
        public void HasAParent()
        {
            Parent parent = new Parent() { UserId = "parentid", Username = "parent", DateOfBirth = DateTime.Today.Date };

            using (MsSqlContext context = NewContext)
            {
                context.Parents.Add(parent);
                context.Children.Add(new Child() { UserId = "childid", Username = "child", DateOfBirth = DateTime.Today.Date, ParentId = "parentid" });

                context.SaveChanges();
            }

            using (MsSqlContext context = NewContext)
            {
                Child child = context.Children.Include(c => c.Parent).FirstOrDefault(c => c.UserId == "childid");

                child.Parent.UserId.Should().Be("parentid");
            }
        }
    }
}
