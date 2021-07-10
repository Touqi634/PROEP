using webApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webApp.Models.EntityMaps;

namespace webApp.Data
{
    public class MySqlContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }

        public MySqlContext(DbContextOptions<MySqlContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserEntityMap());
        }
    }
}
