using Microsoft.EntityFrameworkCore;

using Stott.Optimizely.Csp.Features.Whitelist;

namespace Stott.Optimizely.Csp.Entities
{
    public class CspDataContext : DbContext
    {
        private readonly ICspWhitelistOptions _whitelistOptions;

        public CspDataContext(ICspWhitelistOptions whitelistOptions)
        {
            _whitelistOptions = whitelistOptions;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(_whitelistOptions.ConnectionString);
        }
            
        public DbSet<CspSettings> CspSettings { get; set; }

        public DbSet<CspSource> CspSources { get; set; }

        public DbSet<CspViolationReport> CspViolations { get; set; }

        public DbSet<SecurityHeaderSettings> SecurityHeaderSettings { get; set; }
    }
}
