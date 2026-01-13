namespace Stott.Security.Optimizely.Features.Header;

using System.Collections.Generic;
using System.Threading.Tasks;

using EPiServer.Core;

using Microsoft.AspNetCore.Http;

using Stott.Security.Optimizely.Features.Route;

public interface IHeaderCompilationService
{
    Task<List<HeaderDto>> GetSecurityHeadersAsync(SecurityRouteData routeData, HttpRequest? request);
}
