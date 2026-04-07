using System.Threading.Tasks;

namespace Stott.Security.Optimizely.Features.Csp.Nonce;

public interface INonceProvider
{
    string? GetNonce();

    Task<string?> GetCspValueAsync();
}