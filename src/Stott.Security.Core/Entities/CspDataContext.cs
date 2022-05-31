using System.Threading.Tasks;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Stott.Security.Core.Entities
{
    public class CspDataContext : DbContext, ICspDataContext
    {
        public CspDataContext(DbContextOptions<CspDataContext> options) : base(options)
        {
        }

        public DbSet<CspSettings> CspSettings { get; set; }

        public DbSet<CspSource> CspSources { get; set; }

        public DbSet<CspViolationSummary> CspViolations { get; set; }

        public DbSet<SecurityHeaderSettings> SecurityHeaderSettings { get; set; }

        public async Task<int> ExecuteSqlAsync(string sqlCommand, params SqlParameter[] sqlParameters)
        {
            return await Database.ExecuteSqlRawAsync(sqlCommand, sqlParameters);
        }
    }
}
