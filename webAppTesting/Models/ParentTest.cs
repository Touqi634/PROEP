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
    public class ParentTest : DatabaseTest
    {
        [Fact]
        public void CanHaveManyChildren()
        {
            using (MsSqlContext context = NewContext)
            {
                context.Parents.Add( new Parent() { UserId = "parentid", Username = "mainparent", DateOfBirth = DateTime.Today.Date });
                context.Children.Add( new Child() { UserId = "firstchild", Username = "First child", DateOfBirth = DateTime.Today.Date, ParentId = "parentid" });
                context.Children.Add( new Child() { UserId = "secondchild", Username = "Second child", DateOfBirth = DateTime.Today.Date, ParentId = "parentid" });

                context.SaveChanges();
            }

            using (MsSqlContext context = NewContext)
            {
                Parent parent = context.Parents.Include(p => p.Children).FirstOrDefault(p => p.UserId == "parentid");

                parent.Children.Should().HaveCount(2);
            }
        }
    }
}
