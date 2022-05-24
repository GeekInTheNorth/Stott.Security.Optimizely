using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stott.Optimizely.Csp.Features.Header
{
    public interface ISecurityHeaderService
    {
        Task<Dictionary<string, string>> GetSecurityHeadersAsync();
    }
}
