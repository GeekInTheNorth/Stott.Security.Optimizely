namespace Stott.Security.Optimizely.Features.Header;

using System.Collections.Generic;
using System.Threading.Tasks;

using EPiServer.Core;

using Microsoft.AspNetCore.Http;

public interface IHeaderCompilationService
{
    Task<List<HeaderDto>> GetSecurityHeadersAsync(PageData? pageData, HttpRequest? request);
}
