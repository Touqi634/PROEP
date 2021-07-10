using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace webApp.Models.EntityMaps
{
    public class FriendshipEntityMap : IEntityTypeConfiguration<Friendship>
    {
        public void Configure(EntityTypeBuilder<Friendship> builder)
        {
            // Table and composite key
            builder.ToTable("Friendship")
                .HasKey(f => new { f.UserId, f.FriendId });

            // User relation
            builder.HasOne<User>(f => f.User)
                .WithMany(u => u.Friends)
                .HasForeignKey(f => f.UserId);

            // Friend relation
            builder.HasOne<User>(f => f.Friend)
                .WithMany()
                .HasForeignKey(f => f.FriendId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(f => f.IsBlocked)
                .HasDefaultValue(false);

            //builder.HasData(
            //    new Friendship { UserId = "userid1", FriendId = "userid2"},
            //    new Friendship { UserId = "userid1", FriendId = "userid3"},
            //    new Friendship { UserId = "userid4", FriendId = "userid5"},
            //    new Friendship { UserId = "userid4", FriendId = "userid6"}
            //    );
        }
    }
}
