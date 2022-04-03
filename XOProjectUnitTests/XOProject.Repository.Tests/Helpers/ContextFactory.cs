using Microsoft.EntityFrameworkCore;
using XOProject.Repository;

namespace XOProjectUnitTests.XOProject.Repository.Tests.Helpers
{
    public static class ContextFactory
    {
        public static ExchangeContext CreateContext(bool withData)
        {
            var options = new DbContextOptionsBuilder<ExchangeContext>()
                .UseInMemoryDatabase(databaseName: "ExchangeUnitTestsDb")
                .Options;
            var context = new ExchangeContext(options, new DataSeed());

            if (context != null)
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            return context;
        }
    }
}
