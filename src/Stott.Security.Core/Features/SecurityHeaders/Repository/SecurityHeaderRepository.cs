using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Stott.Security.Core.Entities;
using Stott.Security.Core.Features.SecurityHeaders.Enums;

namespace Stott.Security.Core.Features.SecurityHeaders.Repository
{
    public class SecurityHeaderRepository : ISecurityHeaderRepository
    {
        private readonly ICspDataContext _context;

        public SecurityHeaderRepository(ICspDataContext context)
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
