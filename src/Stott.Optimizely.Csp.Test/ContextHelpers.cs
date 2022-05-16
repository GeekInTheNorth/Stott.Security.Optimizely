using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using Moq;

namespace Stott.Optimizely.Csp.Test
{
    public static class ContextMocker
    {
        public static Mock<DbSet<T>> GetQueryableMockDbSet<T>(List<T> sourceList) where T : class
        {
            var queryable = sourceList.AsQueryable();

            var dbSet = new Mock<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            return dbSet;
        }

        public static Mock<DbSet<T>> GetQueryableMockDbSet<T>(T sourceList) where T : class
        {
            var list = new List<T> { sourceList };

            return GetQueryableMockDbSet(list);
        }

        public static Mock<DbSet<T>> GetQueryableMockDbSet<T>() where T : class
        {
            var list = new List<T>(0);

            return GetQueryableMockDbSet(list);
        }
    }
}
