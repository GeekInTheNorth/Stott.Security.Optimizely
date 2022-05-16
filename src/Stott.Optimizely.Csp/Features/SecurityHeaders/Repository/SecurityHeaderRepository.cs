using System.Linq;
using System.Threading.Tasks;

using Stott.Optimizely.Csp.Entities;
using Stott.Optimizely.Csp.Features.SecurityHeaders.Enums;

namespace Stott.Optimizely.Csp.Features.SecurityHeaders.Repository
{
    public class SecurityHeaderRepository : ISecurityHeaderRepository
    {
        private readonly CspDataContext _context;

        public SecurityHeaderRepository(CspDataContext context)
        {
            _context = context;
        }

        public async Task<SecurityHeaderSettings> GetAsync()
        {
            var settings = await _context.SecurityHeaderSettings.FirstOrDefaultAsync();

            return settings ?? new SecurityHeaderSettings();
        }

        public async Task SaveAsync(bool isXContentTypeOptionsEnabled, bool isXXssProtectionEnabled, ReferrerPolicy referrerPolicy, XFrameOptions frameOptions)
        {
            var recordToSave = await _context.SecurityHeaderSettings.FirstOrDefaultAsync();
            if (recordToSave == null)
            {
                recordToSave = new SecurityHeaderSettings();
                _context.SecurityHeaderSettings.Add(recordToSave);
            }

            recordToSave.IsXContentTypeOptionsEnabled = isXContentTypeOptionsEnabled;
            recordToSave.IsXXssProtectionEnabled = isXXssProtectionEnabled;
            recordToSave.FrameOptions = frameOptions;
            recordToSave.ReferrerPolicy = referrerPolicy;

            await _context.SaveChangesAsync();
        }
    }
}
