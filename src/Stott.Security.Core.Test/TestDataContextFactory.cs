using Microsoft.EntityFrameworkCore;

namespace Stott.Security.Core.Test
{
    public static class TestDataContextFactory
    {
        public static TestDataContext Create()
        {
            var options = new DbContextOptionsBuilder<TestDataContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
            .Options;

            return new TestDataContext(options);
        }
    }
}
