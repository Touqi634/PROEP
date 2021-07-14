using System;
using System.Linq;
using webApp.Models;
using webApp.Models.EntityMaps;
using Microsoft.EntityFrameworkCore;

namespace webApp.Data
{
    public class MsSqlContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Parent> Parents { get; set; }
        public virtual DbSet<Child> Children { get; set; }
        public virtual DbSet<Friendship> Friendships { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<TimeRestriction> TimeRestrictions { get; set; }
        public virtual DbSet<FlaggedMessage> FlaggedMessages { get; set; }

        public MsSqlContext(DbContextOptions<MsSqlContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserEntityMap());
            modelBuilder.ApplyConfiguration(new ParentEntityMap());
            modelBuilder.ApplyConfiguration(new ChildEntityMap());
            modelBuilder.ApplyConfiguration(new FriendshipEntityMap());
            modelBuilder.ApplyConfiguration(new MessageEntityMap());
            modelBuilder.ApplyConfiguration(new TimeRestrictionEntityMap());
            modelBuilder.ApplyConfiguration(new FlaggedMessageEntityMap());
        }

        public override int SaveChanges()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is DataModel && (
                        e.State == EntityState.Added
                        || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((DataModel)entityEntry.Entity).UpdatedAt = DateTime.Now;

                if (entityEntry.State == EntityState.Added)
                {
                    ((DataModel)entityEntry.Entity).CreatedAt = DateTime.Now;
                }
            }

            return base.SaveChanges();
        }
    }
}
