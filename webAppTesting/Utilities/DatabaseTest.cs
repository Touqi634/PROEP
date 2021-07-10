using Microsoft.EntityFrameworkCore;
using webApp.Data;
using System;

namespace webApp.Test.Utilities
{
    public class DatabaseTest
    {
        protected readonly DbContextOptions<MsSqlContext> ContextOptions;

        protected MsSqlContext NewContext { get { return new MsSqlContext(ContextOptions); } }

        public DatabaseTest()
        {
            ContextOptions = new DbContextOptionsBuilder<MsSqlContext>()
                .UseInMemoryDatabase(databaseName: $"inMemoryTestDB_{ Guid.NewGuid()}")
                .Options;

            Seed();
        }

        private void Seed()
        {

            using (var context = NewContext)
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                // If there is something that needs to exist within every test create it here

                context.SaveChanges();
            }
        }
    }
}
