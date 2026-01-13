using System.Collections.Generic;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Features.Header;
using Stott.Security.Optimizely.Features.Route;

namespace Stott.Security.Optimizely.Features.Csp;

public interface ICspService
{
    Task<IEnumerable<HeaderDto>> GetCompiledHeaders(SecurityRouteData routeData);
}