namespace Stott.Security.Optimizely.Features.Csp.Nonce;

public interface INonceProvider
{
    string? GetNonce();

    string? GetCspValue();
}