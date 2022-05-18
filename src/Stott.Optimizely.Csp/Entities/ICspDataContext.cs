using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Stott.Optimizely.Csp.Entities
{
    public interface ICspDataContext
    {
        DbSet<CspSettings> CspSettings { get; set; }

        DbSet<CspSource> CspSources { get; set; }

        DbSet<CspViolationReport> CspViolations { get; set; }

        DbSet<SecurityHeaderSettings> SecurityHeaderSettings { get; set; }

        DatabaseFacade Database { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
