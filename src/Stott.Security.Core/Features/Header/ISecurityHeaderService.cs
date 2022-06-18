namespace Stott.Security.Core.Features.Header;

using System.Collections.Generic;
using System.Threading.Tasks;

public interface ISecurityHeaderService
{
    Task<Dictionary<string, string>> GetSecurityHeadersAsync();
}
