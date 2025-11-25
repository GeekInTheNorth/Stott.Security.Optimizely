using System.Threading.Tasks;

namespace Stott.Security.Optimizely.Features.Csp.Nonce;

public interface INonceService
{
    Task<NonceSettings> GetNonceSettingsAsync();
}
