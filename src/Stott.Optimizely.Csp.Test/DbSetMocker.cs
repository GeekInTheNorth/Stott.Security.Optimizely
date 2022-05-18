using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Microsoft.EntityFrameworkCore;

using Moq;

namespace Stott.Optimizely.Csp.Test
{
    /// <summary>
    /// A helper for mocking DbSets on contexts.
    /// </summary>
    public static class DbSetMocker
    {
        public static Mock<DbSet<T>> GetQueryableMockDbSet<T>(params T[] sourceList) where T : class
        {
            return GetQueryableMockDbSet(sourceList.ToList());
        }

        public static Mock<DbSet<T>> GetQueryableMockDbSet<T>(List<T> sourceList) where T : class
        {
            var queryable = sourceList.AsQueryable();

            var dbSet = new Mock<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
            dbSet.As<IAsyncEnumerable<T>>()
                 .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                 .Returns(new TestAsyncEnumerator<T>(queryable.GetEnumerator()));

            return dbSet;
        }

        public static Mock<DbSet<T>> GetQueryableMockDbSet<T>() where T : class
        {
            var list = new List<T>(0);

            return GetQueryableMockDbSet(list);
        }
    }
}
