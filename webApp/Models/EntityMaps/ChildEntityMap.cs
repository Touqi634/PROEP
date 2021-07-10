using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace webApp.Models.EntityMaps
{
    public class ChildEntityMap : IEntityTypeConfiguration<Child>
    {
        public void Configure(EntityTypeBuilder<Child> builder)
        {
            builder.ToTable("Child");

            //builder.HasData(
            //    new Child { UserId = "userid4", Username = "User4", DateOfBirth = DateTime.Today.Date, Email = "user4@email.com", Phone = null, Bio = "I am the fourth person for testing purposes", ParentId = "userid1" },
            //    new Child { UserId = "userid5", Username = "User5", DateOfBirth = DateTime.Today.Date, Email = "user5@email.com", Phone = null, Bio = "I am the fifth person for testing purposes", ParentId = "userid1" },
            //    new Child { UserId = "userid6", Username = "User6", DateOfBirth = DateTime.Today.Date, Email = "user6@email.com", Phone = null, Bio = "I am the sixth person for testing purposes", ParentId = "userid3" }
            //    );
        }
    }
}
