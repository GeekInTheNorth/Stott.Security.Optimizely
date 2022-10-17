using System.Threading;
using System.Threading.Tasks;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Stott.Security.Optimizely.Entities
{
    public interface ICspDataContext
    {
        DbSet<CspSettings> CspSettings { get; set; }

        DbSet<CspSource> CspSources { get; set; }

        DbSet<CspViolationSummary> CspViolations { get; set; }

        DbSet<SecurityHeaderSettings> SecurityHeaderSettings { get; set; }

        Task<int> ExecuteSqlAsync(string sqlCommand, params SqlParameter[] sqlParameters);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        int SaveChanges();
    }
}
