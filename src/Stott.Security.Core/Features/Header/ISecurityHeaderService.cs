using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stott.Security.Core.Features.Header
{
    public interface ISecurityHeaderService
    {
        Task<Dictionary<string, string>> GetSecurityHeadersAsync();
    }
}
