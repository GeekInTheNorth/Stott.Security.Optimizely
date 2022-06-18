namespace Stott.Security.Core.Features.Header;

using System.Collections.Generic;
using System.Threading.Tasks;

public interface IHeaderCompilationService
{
    Task<Dictionary<string, string>> GetSecurityHeadersAsync();
}
