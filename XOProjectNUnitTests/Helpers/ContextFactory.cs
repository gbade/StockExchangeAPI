using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text;
using XOProject.Repository;

namespace XOProject.UnitTests.Helpers
{
    public static class ContextFactory
    {
        public static ExchangeContext CreateContext(bool withData)
        {
            var options = new DbContextOptionsBuilder<ExchangeContext>()
                .UseInMemoryDatabase("ExchangeUnitTestsDb")
                .Options;
            var context = new ExchangeContext(options, new DataSeed());
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            return context;
        }
    }
}
