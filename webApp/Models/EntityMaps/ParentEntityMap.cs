using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace webApp.Models.EntityMaps
{
    public class ParentEntityMap : IEntityTypeConfiguration<Parent>
    {
        public void Configure(EntityTypeBuilder<Parent> builder)
        {
            builder.ToTable("Parent");

            //builder.HasData(
            //    new Parent { UserId = "userid1", Username = "User1", DateOfBirth = DateTime.Today.Date, Email = "user1@email.com", Phone = null, Bio = "I am the first person for testing purposes" },
            //    new Parent { UserId = "userid2", Username = "User2", DateOfBirth = DateTime.Today.Date, Email = "user2@email.com", Phone = null, Bio = "I am the second person for testing purposes" },
            //    new Parent { UserId = "userid3", Username = "User3", DateOfBirth = DateTime.Today.Date, Email = "user3@email.com", Phone = null, Bio = "I am the third person for testing purposes" }
            //    );
        }
    }
}
