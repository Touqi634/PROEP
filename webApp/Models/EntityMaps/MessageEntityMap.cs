using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace webApp.Models.EntityMaps
{
    public class MessageEntityMap : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("Message")
                .HasKey(m => m.MessageId);

            // Sender relation
            builder.HasOne<User>(s => s.Sender)
                .WithMany(m => m.Messages)
                .HasForeignKey(s => s.SenderID);

            // Receiver relation
            builder.HasOne<User>(r => r.Receiver)
                .WithMany()
                .HasForeignKey(s => s.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
