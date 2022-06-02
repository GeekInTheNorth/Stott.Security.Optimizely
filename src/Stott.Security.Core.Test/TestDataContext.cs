using System.Threading.Tasks;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using Stott.Security.Core.Entities;

namespace Stott.Security.Core.Test
{
    public class TestDataContext : DbContext, ICspDataContext
    {
        public TestDataContext(DbContextOptions<TestDataContext> options) : base(options)
        {
        }

        private int _numberOfRecords = 0;

        public DbSet<CspSettings> CspSettings { get; set; }

        public DbSet<CspSource> CspSources { get; set; }

        public DbSet<CspViolationSummary> CspViolations { get; set; }

        public DbSet<SecurityHeaderSettings> SecurityHeaderSettings { get; set; }

        public void SetExecuteSqlAsyncResult(int numberOfRecords)
        {
            _numberOfRecords = numberOfRecords;
        }

        public Task<int> ExecuteSqlAsync(string sqlCommand, params SqlParameter[] sqlParameters)
        {
            return Task.FromResult(_numberOfRecords);
        }
    }
}
