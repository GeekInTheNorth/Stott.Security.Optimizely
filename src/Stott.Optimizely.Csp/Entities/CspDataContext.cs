
using Microsoft.EntityFrameworkCore;

namespace Stott.Optimizely.Csp.Entities
{
    public class CspDataContext : DbContext, ICspDataContext
    {
        public CspDataContext(DbContextOptions<CspDataContext> options) : base(options)
        {
        }

        public DbSet<CspSettings> CspSettings { get; set; }

        public DbSet<CspSource> CspSources { get; set; }

        public DbSet<CspViolationReport> CspViolations { get; set; }

        public DbSet<SecurityHeaderSettings> SecurityHeaderSettings { get; set; }
    }
}
