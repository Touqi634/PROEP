using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using webApp.Data;
using System;
using Microsoft.Data.Sqlite;

namespace webApp.Test.Utilities
{
    public class MsSqlDatabaseInteractionTest
    {
        protected readonly DbContextOptions<MsSqlContext> ContextOptions;

        public MsSqlDatabaseInteractionTest(DbContextOptions<MsSqlContext> contextOptions)
        {
            ContextOptions = contextOptions;

            Seed();
        }

        private void Seed()
        {
            using (var context = new MsSqlContext(ContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                // If there is something that needs to exist within every test create it here

                context.SaveChanges();
            }
        }
    }
}
