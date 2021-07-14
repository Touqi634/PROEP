using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace webApp.Models.EntityMaps
{
    public class FlaggedMessageEntityMap : IEntityTypeConfiguration<FlaggedMessage>
    {
        public void Configure(EntityTypeBuilder<FlaggedMessage> builder)
        {
            builder.ToTable("FlaggedMessage")
                .HasKey(m => m.FlaggedMessageId);

            // Sender relation
            builder.HasOne<User>(s => s.Sender)
               .WithMany()
               .HasForeignKey(s => s.SenderID);
        }
    }
}
