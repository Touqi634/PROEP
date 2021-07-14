using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace webApp.Models.EntityMaps
{
    public class TimeRestrictionEntityMap : IEntityTypeConfiguration<TimeRestriction>
    {
        public void Configure(EntityTypeBuilder<TimeRestriction> builder)
        {
            builder.ToTable("TimeRestriction")
                .HasKey(tr => tr.RestrictionId);

            // Child relation
            builder.HasOne<Child>(tr => tr.RestrictedUser)
                .WithMany(c => c.TimeRestrictions)
                .HasForeignKey(tr => tr.RestrictedUserId);
        }
    }
}
