using System.Threading.Tasks;
using Stott.Security.Optimizely.Features.Route;

namespace Stott.Security.Optimizely.Features.Csp.Nonce;

public interface INonceService
{
    Task<NonceSettings> GetNonceSettingsAsync(SecurityRouteData? routeData);
}
