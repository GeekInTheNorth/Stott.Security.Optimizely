using System.Collections.Generic;

namespace Stott.Optimizely.Csp.Features.Header
{
    public interface ISecurityHeaderService
    {
        Dictionary<string, string> GetSecurityHeaders();
    }
}
