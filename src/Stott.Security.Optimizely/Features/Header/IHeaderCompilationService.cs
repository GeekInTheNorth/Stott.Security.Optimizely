namespace Stott.Security.Optimizely.Features.Header;

using System.Collections.Generic;
using System.Threading.Tasks;

using EPiServer.Core;

public interface IHeaderCompilationService
{
    Task<Dictionary<string, string>> GetSecurityHeadersAsync(PageData? pageData);
}
