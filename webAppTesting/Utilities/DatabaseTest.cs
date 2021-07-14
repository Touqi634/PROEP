using Microsoft.EntityFrameworkCore;
using webApp.Data;
using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webApp.Mapping;
using webApp.Models;

namespace webApp.Test.Utilities
{
    public class DatabaseTest
    {
        protected readonly DbContextOptions<MsSqlContext> ContextOptions;

        protected MsSqlContext NewContext => new MsSqlContext(ContextOptions);

        protected readonly IMapper mapper;

        public DatabaseTest()
        {
            ContextOptions = new DbContextOptionsBuilder<MsSqlContext>()
                .UseInMemoryDatabase(databaseName: $"inMemoryTestDB_{ Guid.NewGuid()}")
                .Options;

            Seed();

            IConfigurationProvider configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ModelToResourceProfile>();
            });
            mapper = new Mapper(configuration);
        }

        protected void ActingAs(User user,ref ControllerBase controller)
        {
            GenericIdentity myIdentity = new GenericIdentity(user.UserId);
            myIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.UserId));
            var Identity = new ClaimsPrincipal(myIdentity);
            Thread.CurrentPrincipal = Identity;
            controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = Identity };

        }

        private void Seed()
        {
            using var context = NewContext;
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // If there is something that needs to exist within every test create it here

            context.SaveChanges();
        }
    }
}
